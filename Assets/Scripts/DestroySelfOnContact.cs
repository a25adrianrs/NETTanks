using UnityEngine;

/// <summary>
/// Este script destruye el objeto cuando entra en contacto (colisiona) con otro objeto.
/// Se utiliza para proyectiles, explosiones u otros objetos que desaparecen tras chocar.
/// </summary>
public class DestroySelfOnContact : MonoBehaviour
{
    /// <summary>
    /// Se ejecuta automáticamente cuando un collider configurado como trigger entra en contacto con otro collider.
    /// El parámetro 'collision' contiene información del objeto con el que se ha chocado.
    /// </summary>
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Destruye el objeto actual (gameObject) del que es propietario este script
        Destroy(gameObject);
    }
}
