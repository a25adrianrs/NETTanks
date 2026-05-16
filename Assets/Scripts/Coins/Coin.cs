using Unity.Netcode;
using UnityEngine;

/// <summary>
<<<<<<< HEAD
/// Clase abstracta que representa una moneda en el juego.
/// Define la estructura básica y el comportamiento común de todas las monedas,
/// pero no implementa la lógica específica de recolección, que queda a cargo de las clases derivadas.
/// Las clases "abstractas" sirven como plantillas que deben ser heredadas por otras clases.
/// </summary>
public abstract class Coin : NetworkBehaviour
{
    /// <summary>
    /// Referencia al componente SpriteRenderer de la moneda.
    /// Se utiliza para mostrar u ocultar la moneda visualmente en el juego.
    /// </summary>
=======
/// Define la funcionalidad básica de una moneda en el juego.
/// Las clases derivadas deben implementar la lógica concreta de recolección.
/// </summary>
public abstract class Coin : NetworkBehaviour
{
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
    [SerializeField] private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Valor que otorga la moneda cuando es recogida.
    /// Se puede cambiar con SetValue() antes de que sea recolectada.
    /// Este valor se suma al monedero del jugador al recoger la moneda.
    /// </summary>
    protected int coinValue = 10;

    /// <summary>
    /// Bandera para rastrear si esta moneda ya ha sido recolectada.
    /// Evita que se cuente la misma moneda múltiples veces.
    /// </summary>
    protected bool alreadyCollected;

<<<<<<< HEAD
    /// <summary>
    /// Método abstracto que DEBE ser implementado por las clases que heredan de esta.
    /// Define qué sucede cuando la moneda es recolectada.
    /// Las clases derivadas (como RespawningCoin) implementan esta lógica específica.
    /// Devuelve el valor de monedas que otorga al ser recogida.
    /// </summary>
    public abstract int Collect();

    /// <summary>
    /// Método público para establecer el valor de la moneda.
    /// Se llama antes de spawnear la moneda para configurar cuánto vale.
    /// Permite reutilizar el prefab de moneda con diferentes valores.
    /// </summary>
=======
    public abstract int Collect();

>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
    public void SetValue(int value)
    {
        // Asigna el valor pasado como parámetro al atributo coinValue
        coinValue = value;
    }

<<<<<<< HEAD
    /// <summary>
    /// Método protegido para mostrar u ocultar la moneda visualmente.
    /// "Protegido" significa que solo esta clase y sus derivadas pueden usarlo.
    /// Se utiliza para crear el efecto visual de "recolectada" o "respawneda".
    /// </summary>
=======
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
    protected void Show(bool show)
    {
        // Activa o desactiva el SpriteRenderer según el parámetro 'show'
        // true = mostrar la moneda, false = ocultarla
        spriteRenderer.enabled = show;
    }
}
