using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Este script gestiona la generación y reaparición de monedas en el mapa en multiplicador.
/// Solo el servidor genera las monedas en posiciones aleatorias válidas (sin colisiones).
/// Cuando una moneda es recolectada, se mueve a una nueva posición y vuelve a aparecer.
/// </summary>
public class CoinSpawner : NetworkBehaviour
{
    /// <summary>
    /// Prefab de la moneda que se instanciará múltiples veces.
    /// Contiene el componente RespawningCoin con toda la lógica de moneda.
    /// </summary>
    [SerializeField] private RespawningCoin coinPrefab;

    /// <summary>
    /// Número máximo de monedas que pueden existir simultáneamente en el mapa.
    /// Más monedas = más oportunidades de ganar dinero pero más objetos en memoria.
    /// </summary>
    [SerializeField] private int maxCoins = 50;

    /// <summary>
    /// Valor de cada moneda cuando es recolectada.
    /// Se asigna a cada moneda spawneada para que todas tengan el mismo valor.
    /// </summary>
    [SerializeField] private int coinValue = 10;

    /// <summary>
    /// Rango de posiciones en el eje X donde pueden aparecer las monedas.
    /// xSpawnRange.x = posición X mínima, xSpawnRange.y = posición X máxima.
    /// Limita el área de generación del mapa.
    /// </summary>
    [SerializeField] private Vector2 xSpawnRange;

    /// <summary>
    /// Rango de posiciones en el eje Y donde pueden aparecer las monedas.
    /// ySpawnRange.x = posición Y mínima, ySpawnRange.y = posición Y máxima.
    /// Limita el área de generación del mapa verticalmente.
    /// </summary>
    [SerializeField] private Vector2 ySpawnRange;

    /// <summary>
    /// Máscara de capas que se evitan al generar monedas.
    /// Se utiliza para detectar colisiones y no spawnear monedas dentro de obstáculos.
    /// Se asigna desde el Inspector (típicamente capas como "Paredes" o "Obstáculos").
    /// </summary>
    [SerializeField] private LayerMask layerMask;

    /// <summary>
    /// Buffer para almacenar resultados de detección de colisiones.
    /// Se reutiliza para evitar crear nuevas arrays cada frame (mejor rendimiento).
    /// Tamaño 1 porque solo necesitamos saber si hay algo en el punto (sí o no).
    /// </summary>
    private Collider2D[] coinBuffer = new Collider2D[1];

    /// <summary>
    /// Radio del collider de la moneda.
    /// Se obtiene del prefab y se usa para comprobar si hay espacio para spawnear.
    /// Las monedas son círculos, así que tienen un radio (no ancho/alto).
    /// </summary>
    private float coinRadius;

    /// <summary>
    /// Se ejecuta cuando el objeto aparece en la red.
    /// Solo el servidor debe generar las monedas iniciales.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        // Solo el servidor genera las monedas
        // Los clientes las reciben automáticamente del servidor
        if (!IsServer) { return; }

        // Obtiene el radio del collider circular de la moneda desde el prefab
        // Necesario para comprobar que hay espacio disponible al spawnear
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        // Genera la cantidad máxima de monedas configuradas
        // Cada una se spawnea en una posición válida (sin colisiones)
        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    /// <summary>
    /// Crea una nueva moneda en el servidor en una posición válida (sin colisiones).
    /// La moneda se spawnea en la red para que todos los clientes la vean.
    /// Se suscribe al evento de recolección para poder reaparición cuando se recoge.
    /// </summary>
    private void SpawnCoin()
    {
        // Instancia una moneda en una posición válida obtenida por GetSpawnPoint()
        // Quaternion.identity = sin rotación (rotación neutra)
        RespawningCoin coinInstance = Instantiate(
            coinPrefab,
            GetSpawnPoint(),
            Quaternion.identity);

        // Asigna el valor configurado a la moneda
        // Todas las monedas en este spawner tienen el mismo valor
        coinInstance.SetValue(coinValue);

        // Spawnea la moneda en la red de Netcode
        // Esto sincroniza la creación de la moneda con todos los clientes conectados
        coinInstance.GetComponent<NetworkObject>().Spawn();

        // Se suscribe al evento de recolección de la moneda
        // Cuando la moneda es recogida, HandleCoinCollected se ejecutará automáticamente
        // Permite que el spawner sepa cuándo reaparición la moneda
        coinInstance.OnCollected += HandleCoinCollected;
    }

    /// <summary>
    /// Se ejecuta automáticamente cuando una moneda es recolectada por un jugador.
    /// Mueve la moneda a una nueva posición y la reinicia para que pueda ser recolectada de nuevo.
    /// </summary>
    private void HandleCoinCollected(RespawningCoin coin)
    {
        // Cuando una moneda se recoge:
        // - Se mueve a otra posición aleatoria
        // - Se reinicia (la bandera alreadyCollected se pone a false)
        // Asigna una nueva posición aleatoria válida a la moneda
        coin.transform.position = GetSpawnPoint();

        // Reinicia la moneda para que pueda ser recolectada nuevamente
        // Reset() pone alreadyCollected en false y la vuelve visible
        coin.Reset();
    }

    /// <summary>
    /// Genera una posición aleatoria válida donde spawnear una moneda.
    /// Busca una posición dentro de los rangos X,Y que no tenga colisiones.
    /// Mantiene un bucle hasta encontrar un espacio libre.
    /// </summary>
    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;

        // Búsqueda infinita hasta encontrar una posición válida (sin colisiones)
        while (true)
        {
            // Genera una posición X aleatoria dentro del rango especificado
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);

            // Genera una posición Y aleatoria dentro del rango especificado
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);

            // Crea un Vector2 con las coordenadas generadas
            Vector2 spawnPoint = new Vector2(x, y);

            // Verifica si hay colisiones en ese punto
            // OverlapCircleNonAlloc es más eficiente que OverlapCircle porque reutiliza el array
            // Parámetros: punto de prueba, radio de búsqueda, array de resultados, capas a comprobar
            // Devuelve el número de colisores encontrados
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);

            // Si no hay colisiones (numColliders == 0), la posición es válida
            if (numColliders == 0)
            {
                // Devuelve esta posición para spawnear la moneda
                return spawnPoint;
            }
            // Si hay colisiones, el bucle while continúa y genera otra posición aleatoria
        }
    }
}
