using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    // Guarda el total de monedas recolectadas por el jugador, 
    // sincronizado en red para que todos los clientes tengan la misma información.
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    // Se gastan monedas para disparar, restando el costo al total de monedas del jugador.
    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }


    // Si el jugador entra en contacto con una moneda, 
    // se intenta recolectar la moneda y se suma su valor al total de monedas del jugador.
    private void OnTriggerEnter2D(Collider2D col)
    {
        // Si no se puede obtener el componente Coin del objeto con el que se colisionó, no se hace nada (no es una moneda).
        if (!col.TryGetComponent<Coin>(out Coin coin)) { return; }
        // Si se obtiene la moneda, se llama a su método Collect para recolectarla y obtener su valor.
        int coinValue = coin.Collect();
        // Si no es el servidor, no se actualiza el total de monedas, 
        // ya que solo el servidor debe manejar la lógica de recolección y actualización de monedas.
        if (!IsServer) { return; }

        // Si es el servidor, se suma el valor de la moneda al total de monedas del jugador.
        TotalCoins.Value += coinValue;
    }
}
