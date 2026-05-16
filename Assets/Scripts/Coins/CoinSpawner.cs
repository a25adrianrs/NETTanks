using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Genera monedas en el mapa y las recoloca cuando son recogidas.
/// El servidor crea las monedas en red para que todos los clientes las vean.
/// </summary>
public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin coinPrefab; // Prefab de la moneda

    [SerializeField] private int maxCoins = 50;// Número máximo de monedas en el mapa
    [SerializeField] private int coinValue = 10;// Valor de cada moneda
    [SerializeField] private Vector2 xSpawnRange; // Rango de posiciones en X donde pueden aparecer
    [SerializeField] private Vector2 ySpawnRange; // Rango de posiciones en Y donde pueden aparecer
    [SerializeField] private LayerMask layerMask;   // Capas a evitar al generar monedas (colisiones)

    private Collider2D[] coinBuffer = new Collider2D[1]; // Buffer para detectar colisiones
    private float coinRadius; // Radio de la moneda para comprobar espacio libre

    public override void OnNetworkSpawn()
    {
        // Solo el servidor genera las monedas
        if (!IsServer) { return; }

        // Obtiene el radio del collider de la moneda
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;
        // Genera tantas monedas como el máximo definido
        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        // Instancia una moneda en una posición válida
        RespawningCoin coinInstance = Instantiate(
            coinPrefab,
            GetSpawnPoint(),
            Quaternion.identity);

        // Asigna el valor de la moneda
        coinInstance.SetValue(coinValue);
        // La spawnea en red para que todos los clientes la vean
        coinInstance.GetComponent<NetworkObject>().Spawn();

        // Se dispara el evento cuando la moneda es recogida para recolocarla
        coinInstance.OnCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        // Cuando una moneda se recoge:
        // - se mueve a otra posición
        // - se reinicia (vuelve a aparecer)
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;
        // Busca una posición aleatoria válida (sin colisiones)
        while (true)
        {
            // Genera posición aleatoria dentro de los rangos
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPoint = new Vector2(x, y);
            // Comprueba si hay colisiones en ese punto
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            // Si no hay colisiones, devuelve esa posición
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
