using UnityEngine;
using Unity.Netcode;
public class CameraFollow : MonoBehaviour
{
    private Transform target;
    void LateUpdate()
    {
        // Si no tenemos target, no hacemos nada
        if (target == null) return;

        // Mantiene la Z de la cámara 
        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );
    }

    // Método para asignar el jugador a seguir
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
