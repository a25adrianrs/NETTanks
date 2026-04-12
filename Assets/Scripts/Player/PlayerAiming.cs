// Es necesario que Unity.Netcode esté presente si queremos usar las funcionalidades de red, como verificar la propiedad del objeto
using Unity.Netcode;
using UnityEngine;

//El script debe ser un NetworkBehavioour y no un MonoBehaviour para poder usar las funcionalidades de red.
public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;


    // Metodo que se llama cuando el objeto se crea el objeto en la red;
    // Hace que la torreta apunte hacia el ratón del jugador,
    // calculando la dirección desde la torreta hasta la posición del cursor
    private void LateUpdate()
    {
        if (!IsOwner) { return; }

        // Obtener la posición del mouse en pantalla desde el InputReader
        Vector2 aimScreenPosition = inputReader.AimPosition;
        // Convertir la posición del mouse de coordenadas de pantalla a coordenadas del mundo
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        // Calcular la dirección desde el cañón del tanque (turretTransform) hacia la posición del mouse en el mundo
        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y
        );
    }
}
