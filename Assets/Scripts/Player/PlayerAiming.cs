// Es necesario que Unity.Netcode esté presente si queremos usar las funcionalidades de red, como verificar la propiedad del objeto
using Unity.Netcode;
using UnityEngine;

<<<<<<< HEAD
/// <summary>
/// Este script hace que la torreta (cañón) del tanque apunte hacia la posición del ratón del jugador.
/// Sincroniza la rotación de la torreta para todos los jugadores en la red.
/// Cada jugador solo controla la puntería de su propio tanque.
=======
//El script debe ser un NetworkBehavioour y no un MonoBehaviour para poder usar las funcionalidades de red.
/// <summary>
/// Hace que la torreta del jugador apunte hacia el cursor del jugador local.
/// Solo el propietario del objeto actualiza la rotación.
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
/// </summary>
public class PlayerAiming : NetworkBehaviour
{
    /// <summary>
    /// Referencia al componente InputReader que proporciona las entradas del jugador.
    /// Se utiliza para obtener la posición actual del ratón/cursor en la pantalla.
    /// </summary>
    [SerializeField] private InputReader inputReader;

    /// <summary>
    /// Referencia al Transform de la torreta del tanque.
    /// Es el objeto que rota para apuntar hacia donde está el ratón.
    /// Se asigna desde el Inspector de Unity.
    /// </summary>
    [SerializeField] private Transform turretTransform;

<<<<<<< HEAD
    /// <summary>
    /// Se ejecuta en LateUpdate (después de que otros scripts hayan actualizado sus valores).
    /// Es ideal para calcular rotaciones de cámara y orientación basadas en entrada del jugador.
    /// En este caso, LateUpdate asegura que el InputReader ya ha actualizado la posición del ratón.
    /// </summary>
=======
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
    private void LateUpdate()
    {
        // Verifica si este objeto es propiedad del jugador local
        // Solo el dueño del objeto puede controlar su puntería
        // Esto evita que otros clientes controlen tu tanque
        if (!IsOwner) { return; }

        // Obtener la posición del mouse en pantalla desde el InputReader
        // AimPosition devuelve las coordenadas X,Y del cursor en la pantalla (en píxeles)
        Vector2 aimScreenPosition = inputReader.AimPosition;

        // Convertir la posición del mouse de coordenadas de pantalla a coordenadas del mundo 3D
        // ScreenToWorldPoint transforma píxeles a posición en el mundo del juego
        // Camera.main accede a la cámara principal de la escena
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        // Calcula la dirección desde el cañón del tanque (turretTransform) hacia la posición del mouse en el mundo
        // Restamos la posición del cañón de la posición del ratón para obtener el vector de dirección
        // turretTransform.up = asigna el vector calculado como la dirección "hacia arriba" (forward) de la torreta
        // Esto hace que el cañón apunte hacia el ratón instantáneamente
        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y
        );
    }
}
