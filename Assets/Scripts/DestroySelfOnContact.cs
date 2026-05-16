using UnityEngine;

/// <summary>
<<<<<<< HEAD
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
=======
/// Destruye este objeto cuando entra en contacto con otro collider.
/// Se usa típicamente en proyectiles o efectos que deben desaparecer al chocar.
/// </summary>
public class DestroySelfOnContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
    {
        // Destruye el objeto actual (gameObject) del que es propietario este script
        Destroy(gameObject);
    }
}
