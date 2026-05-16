using UnityEngine;

/// <summary>
/// Destruye este objeto cuando entra en contacto con otro collider.
/// Se usa típicamente en proyectiles o efectos que deben desaparecer al chocar.
/// </summary>
public class DestroySelfOnContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
