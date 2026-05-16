using System;
using UnityEngine;

/// <summary>
/// Moneda que puede reaparecer después de ser recogida.
/// Notifica al spawner mediante OnCollected para que vuelva a colocarse en el mapa.
/// </summary>
public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;
    private Vector3 previousPosition;

    private void Update()
    {
        if (previousPosition != transform.position)
        {
            Show(true);
        }
        previousPosition = transform.position;
    }

    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if (alreadyCollected) { return 0; }
        alreadyCollected = true;
        OnCollected?.Invoke(this);
        return coinValue;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
