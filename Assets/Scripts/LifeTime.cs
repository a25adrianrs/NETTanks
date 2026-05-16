using UnityEngine;

/// <summary>
<<<<<<< HEAD
/// Este script destruye automáticamente el objeto después de un tiempo específico.
/// Es útil para efectos visuales, partículas o cualquier objeto temporal que debe desaparecer automáticamente.
=======
/// Destruye este objeto automáticamente después de un tiempo definido.
/// Se usa en objetos temporales como proyectiles, efectos y partículas.
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
/// </summary>
public class LifeTime : MonoBehaviour
{
    /// <summary>
    /// Tiempo en segundos antes de que el objeto sea destruido.
    /// Se puede ajustar desde el Inspector de Unity.
    /// </summary>
    [SerializeField] private float lifeTime = 1f;
<<<<<<< HEAD

    void Start()
    {
        // Se programa la ejecución del método DestroyAfterTime después de 'lifeTime' segundos
        // Esto es más eficiente que usar Update para contar tiempo en este caso
        Invoke("DestroyAfterTime", lifeTime);
    }

    /// <summary>
    /// Método privado que se ejecuta automáticamente después del tiempo especificado en Start().
    /// Destruye el objeto cuando se alcanza su tiempo de vida.
    /// </summary>
    private void DestroyAfterTime()
    {
        // Destruye el GameObject (el objeto completo con todos sus componentes)
        Destroy(gameObject);
=======

    private void Start()
    {
        Destroy(gameObject, lifeTime);
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
    }
}
