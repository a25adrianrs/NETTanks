using UnityEngine;

/// <summary>
/// Destruye este objeto automáticamente después de un tiempo definido.
/// Se usa en objetos temporales como proyectiles, efectos y partículas.
/// </summary>
public class LifeTime : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
