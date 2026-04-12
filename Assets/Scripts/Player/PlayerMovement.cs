// Es necesario que Unity.Netcode esté presente si queremos usar las funcionalidades de red, 
// como usar los metodos OnNetworkSpawn y OnNetworkDespawn para manejar la suscripción a eventos 
// de entrada solo para el propietario del objeto.
using Unity.Netcode;
using UnityEngine;

// Ademas el script debe ser un NetworkBehavioour y no un MonoBehaviour para poder usar las funcionalidades de red.
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

    // Metodo que se llama cuando el objeto se crea el objeto en la red;
    public override void OnNetworkSpawn()
    { // Verificar si el objeto es propiedad del cliente local antes de suscribirse al evento
        if (!IsOwner) return;
        // Suscribirse al evento de movimiento del InputReader
        // Esto permite que el método HandleMove se llame cada vez que el jugador proporciona una entrada de movimiento
        inputReader.MoveEvent += HandleMove;
    }


    // Metodo que se llama cuando el objeto se destruye en la red
    public override void OnNetworkDespawn()
    {
        // Verificar si el objeto es propiedad del cliente local antes de desuscribirse del evento
        if (!IsOwner) return;
        // Desuscribirse del evento de movimiento del InputReader para evitar 
        // llamadas a métodos después de que el objeto haya sido destruido
        inputReader.MoveEvent -= HandleMove;
    }

    // En el método Update, se aplica la rotación al cuerpo del jugador 
    //en función de la entrada de movimiento anterior y la tasa de giro. En el método FixedUpdate, 
    // se actualiza la velocidad lineal del Rigidbody2D para mover al jugador en la dirección que 
    // está mirando, multiplicando la velocidad de movimiento por la entrada de movimiento anterior y 
    // la dirección hacia adelante del cuerpo del jugador.
    void Update()
    {
        //El codigo solo se ejecuta para el propietario del objeto, 
        // evitando que otros clientes puedan controlar el movimiento de este jugador. 
        // Si el objeto no es propiedad del cliente local,
        if (!IsOwner) return;
        // // Calcula cuánto debe girar el tanque en este frame según el input horizontal,
        // la velocidad de giro y el tiempo entre frames (para que el movimiento sea suave)
        float zRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);

    }

    // En el método Update, se aplica la rotación al cuerpo del jugador
    void FixedUpdate()
    {
        if (!IsOwner) return;
        // Mueve el tanque hacia delante o atrás según el input vertical,
        // usando la dirección actual del tanque (hacia donde está mirando)
        rb.linearVelocity = movementSpeed * previousMovementInput.y * (Vector2)bodyTransform.up;

    }

    // Método que maneja la entrada de movimiento del jugador, 
    // actualizando la variable previousMovementInput con el valor de la entrada de movimiento recibida.
    private void HandleMove(Vector2 movementInput)
    {
        // Actualizar la variable previousMovementInput con el valor de la 
        // entrada de movimiento recibida
        previousMovementInput = movementInput;
    }
}
