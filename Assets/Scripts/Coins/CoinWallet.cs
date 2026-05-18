using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Este script gestiona el dinero (monedas) del jugador en el juego multiplicador.
/// Sincroniza el balance de monedas a través de la red para que todos vean el mismo valor.
/// Las monedas se gastan para disparar y se ganan recolectando monedas del mapa.
/// </summary>
public class CoinWallet : NetworkBehaviour
{
    /// <summary>
    /// Total de monedas que tiene el jugador, sincronizado a través de la red.
    /// NetworkVariable<int> asegura que todos los jugadores vean el mismo valor actualizado.
    /// Es esencial para el multiplicador donde el dinero es compartido por todos.
    /// </summary>
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    /// <summary>
    /// Se ejecuta cuando el jugador gasta monedas (por ejemplo, para disparar).
    /// Resta la cantidad especificada del total de monedas del jugador.
    /// </summary>
    public void SpendCoins(int costToFire)
    {
        // Resta el costo del disparo del total de monedas del jugador
        // Por ejemplo: si costToFire = 5 y TotalCoins = 20, después será 15
        TotalCoins.Value -= costToFire;
    }

    /// <summary>
    /// Se ejecuta automáticamente cuando el jugador entra en contacto (colisiona) con una moneda.
    /// Es el mecanismo por el cual el jugador recolecta monedas del mapa.
    /// Solo el servidor puede actualizar el total de monedas.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D col)
    {
        // Intenta obtener el componente Coin del objeto con el que se colisionó
        // Si el objeto no es una moneda (no tiene el componente Coin), sale del método
        if (!col.TryGetComponent<Coin>(out Coin coin)) { return; }

        // Llama al método Collect() de la moneda para obtener su valor
        // La moneda decide cuánto vale (puede estar vacía si ya fue recogida)
        int coinValue = coin.Collect();

        // Si este objeto no es el servidor, no puede actualizar las monedas
        // Los clientes no pueden modificar valores de red (solo el servidor puede)
        // Sin esta verificación, se podrían hacer trucos para ganar dinero infinito
        if (!IsServer) { return; }

        // Suma el valor de la moneda recolectada al total de monedas del jugador
        // Esto sincroniza automáticamente con todos los clientes conectados
        TotalCoins.Value += coinValue;
    }
}
