using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Almacena y sincroniza las monedas recolectadas por el jugador.
/// Solo el servidor actualiza el total; los clientes reciben el valor por red.
/// </summary>
public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent<Coin>(out Coin coin)) { return; }

        int coinValue = coin.Collect();
        if (!IsServer) { return; }

        TotalCoins.Value += coinValue;
    }
}
