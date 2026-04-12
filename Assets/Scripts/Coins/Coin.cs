using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Clase abstracta que representa una moneda en el juego. Define la estructura básica y el comportamiento común de todas las monedas, 
// pero no implementa la lógica específica de recolección, que queda a cargo de las clases derivadas.
public abstract class Coin : NetworkBehaviour
{
    // Referencia al SpriteRenderer para mostrar u ocultar la moneda visualmente
    [SerializeField] private SpriteRenderer spriteRenderer;

    protected int coinValue = 10;
    protected bool alreadyCollected;

    // Método abstracto que debe ser implementado por las clases derivadas para definir la lógica de recolección de la moneda.
    public abstract int Collect();

    // Método para establecer el valor de la moneda, que puede ser llamado por otros scripts para configurar 
    // la moneda antes de que sea recolectada.
    public void SetValue(int value)
    {
        coinValue = value;
    }

    // Método protegido para mostrar u ocultar la moneda visualmente, 
    // utilizado por las clases derivadas para controlar la apariencia de la moneda en el juego.
    protected void Show(bool show)
    {
        spriteRenderer.enabled = show;
    }
}
