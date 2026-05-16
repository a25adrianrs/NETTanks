using UnityEngine;

// Este script crea una nueva instancia de un prefab cuando el objeto se destruye.
// Es útil para efectos de destrucción, enemigos que dejan algo al morir o reemplazos visuales.
/// <summary>
/// Instancia un prefab en la posición de este objeto cuando se destruye.
/// Puede utilizarse para reemplazar un objeto con un efecto, enemigo o animación.
/// </summary>
public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    // Mediante la función Instantiate, se crea una nueva instancia del prefab 
    // en la posición del objeto actual (transform.position) 
    // y con una rotación de identidad (Quaternion.identity).
    // Esto significa que cada vez que este objeto sea destruido, 
    // se generará una nueva instancia del prefab en su lugar.
    private void OnDestroy()
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
