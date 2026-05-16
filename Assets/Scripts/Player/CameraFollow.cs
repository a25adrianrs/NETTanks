using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Este script hace que la cámara siga suavemente al jugador durante el juego.
/// Sincroniza la posición de la cámara con el jugador en los ejes X e Y, 
/// manteniendo la distancia Z (profundidad) constante para garantizar la vista 2D correcta.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// Referencia al Transform del objeto a seguir (típicamente el jugador principal).
    /// Si es null, la cámara permanecerá en su posición actual.
    /// Se asigna dinámicamente mediante el método SetTarget().
    /// </summary>
    private Transform target;

    /// <summary>
    /// LateUpdate se ejecuta después de Update, es ideal para cálculos de cámara
    /// para garantizar que el jugador se ha movido antes de actualizar la cámara.
    /// </summary>
    void LateUpdate()
    {
        // Si no tenemos target (objeto a seguir), no hacemos nada
        // Esto evita errores si la cámara se ejecuta antes de que se asigne un objetivo
        if (target == null) return;

        // Actualiza la posición de la cámara para seguir al jugador
        // Copia la posición X e Y del objetivo para mantener la cámara centrada
        // Mantiene la Z de la cámara sin cambios para preservar la profundidad de la vista 2D
        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );
    }

    /// <summary>
    /// Método público para asignar el jugador a seguir.
    /// Se llama desde otros scripts (como PlayerMovement) para establecer el objetivo de seguimiento.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        // Guarda la referencia al nuevo objetivo para seguir en LateUpdate
        target = newTarget;
    }
}
