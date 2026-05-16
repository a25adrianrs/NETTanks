using UnityEngine;

<<<<<<< HEAD
/// <summary>
/// Este script crea una copia (instancia) de un prefab en la posición actual del objeto 
/// cada vez que el objeto es destruido.
/// Útil para crear efectos de explosión, dejar rastros, o generar nuevos objetos al destruirse.
=======
// Este script crea una nueva instancia de un prefab cuando el objeto se destruye.
// Es útil para efectos de destrucción, enemigos que dejan algo al morir o reemplazos visuales.
/// <summary>
/// Instancia un prefab en la posición de este objeto cuando se destruye.
/// Puede utilizarse para reemplazar un objeto con un efecto, enemigo o animación.
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
/// </summary>
public class SpawnOnDestroy : MonoBehaviour
{
    /// <summary>
    /// El prefab que se instanciará cuando este objeto sea destruido.
    /// Un prefab es una plantilla reutilizable de un GameObject configurada previamente.
    /// Se asigna desde el Inspector de Unity.
    /// </summary>
    [SerializeField] private GameObject prefab;
<<<<<<< HEAD

    /// <summary>
    /// Se ejecuta automáticamente cuando el objeto está a punto de ser destruido.
    /// Es el momento perfecto para realizar acciones finales como generar efectos o spawneando nuevos objetos.
    /// </summary>
    private void OnDestroy()
    {
        // Mediante la función Instantiate, se crea una nueva instancia del prefab 
        // en la posición del objeto actual (transform.position) 
        // y con una rotación de identidad (Quaternion.identity).
        // transform.position = ubicación en el mundo donde se spawneará el prefab
        // Quaternion.identity = rotación neutra (0,0,0) - sin rotación aplicada
        // Esto significa que cada vez que este objeto sea destruido, 
        // se generará una nueva instancia del prefab en su lugar.
=======
    // Mediante la función Instantiate, se crea una nueva instancia del prefab 
    // en la posición del objeto actual (transform.position) 
    // y con una rotación de identidad (Quaternion.identity).
    // Esto significa que cada vez que este objeto sea destruido, 
    // se generará una nueva instancia del prefab en su lugar.
    private void OnDestroy()
    {
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
