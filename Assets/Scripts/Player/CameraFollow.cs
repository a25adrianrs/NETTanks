using UnityEngine;

/// <summary>
/// Sigue a un objetivo en el mundo manteniendo la posición de la cámara en X e Y.
/// No cambia la profundidad (Z) para mantener la vista en 2D.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    private Transform target;

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );
    }

    /// <summary>
    /// Asigna el Transform del jugador que la cámara debe seguir.
    /// Normalmente es llamado desde PlayerMovement cuando el jugador local se inicializa.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
