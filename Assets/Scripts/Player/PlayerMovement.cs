using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Controla el movimiento del jugador local y asigna la cámara al jugador propietario.
/// Solo el cliente que es dueño de este objeto procesa la entrada.
/// </summary>
public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]

    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float turningRate = 30f;

    private Vector2 previousMovementInput;

    public override void OnNetworkSpawn()
    {
        // Verificar si el objeto es propiedad del cliente local antes de suscribirse al evento
        if (!IsOwner) return;

        // Suscribirse al evento de movimiento del InputReader
        // Esto permite que el método HandleMove se llame cada vez que el jugador proporciona una entrada de movimiento
        inputReader.MoveEvent += HandleMove;

        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(transform);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent -= HandleMove;
    }

    private void Update()
    {
        if (!IsOwner) return;
        // Calcula cuánto debe girar el tanque en este frame según el input horizontal,
        // la velocidad de giro y el tiempo entre frames (para que el movimiento sea suave)
        float zRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        // Mueve el tanque hacia delante o atrás según el input vertical,
        // usando la dirección actual del tanque (hacia donde está mirando)
        rb.linearVelocity = movementSpeed * previousMovementInput.y * (Vector2)bodyTransform.up;
    }

    private void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }
}
