using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Define la funcionalidad básica de una moneda en el juego.
/// Las clases derivadas deben implementar la lógica concreta de recolección.
/// </summary>
public abstract class Coin : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    protected int coinValue = 10;
    protected bool alreadyCollected;

    public abstract int Collect();

    public void SetValue(int value)
    {
        coinValue = value;
    }

    protected void Show(bool show)
    {
        spriteRenderer.enabled = show;
    }
}
