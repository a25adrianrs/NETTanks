using Unity.Netcode;
using UnityEngine;


// Gestiona el disparo del jugador en multiplayer:
// crea un disparo visual inmediato en el cliente y el proyectil real en el servidor,
// sincronizándolo con todos los jugadores para que vean el disparo al mismo tiempo.
public class ProjectileLauncher : NetworkBehaviour
{

    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private CoinWallet wallet;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;
    [SerializeField] private int costToFire;


    private bool shouldFire;
    private float timer;
    private float muzzleFlashTimer;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;

    }

    // En el método Update, se maneja el temporizador para el efecto de fogonazo 
    //de la boca del cañón (muzzle flash) y se verifica si el jugador ha 
    //intentado disparar. Si el jugador ha intentado disparar, se verifica si el temporizador de disparo 
    //ha expirado y si el jugador tiene suficientes monedas para disparar. Si todas las condiciones se cumplen, 
    //se llama al método PrimaryFireServerRpc para instanciar el proyectil en el servidor y se actualiza el temporizador de disparo.
    void Update()
    {
        // Si el temporizador del fogonazo es mayor que cero, se reduce el 
        // temporizador en función del tiempo transcurrido desde el último frame.
        if (muzzleFlashTimer > 0f)
        {
            // Reducir el temporizador del fogonazo en función del tiempo transcurrido desde el último frame
            muzzleFlashTimer -= Time.deltaTime;
            // Si el temporizador del fogonazo ha expirado, se desactiva el objeto del fogonazo para ocultarlo visualmente.
            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) { return; }
        // Si el temporizador de disparo es mayor que cero, 
        // se reduce el temporizador en función del tiempo transcurrido desde el último frame.
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        // Si el jugador no ha intentado disparar, se sale del método para evitar ejecutar el código de disparo.
        if (!shouldFire) { return; }
        // Verificar si el temporizador de disparo ha expirado antes de permitir otro disparo
        if (timer > 0) { return; }

        if (wallet.TotalCoins.Value < costToFire) { return; }
        // Llamar al método PrimaryFireServerRpc para instanciar el proyectil en el servidor, 
        // pasando la posición y dirección del cañón del tanque.
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        // Llamar al método SpawnDummyProjectile para crear un proyectil de "disparo simulado" en el cliente,
        // proporcionando retroalimentación visual inmediata al jugador que dispara.
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        // Actualizar el temporizador de disparo para controlar la tasa de fuego.
        timer = 1 / fireRate;

    }

    // El método SpawnDummyProjectile se encarga de instanciar un proyectil de "disparo simulado" 
    //en el cliente para proporcionar retroalimentación visual inmediata al jugador que dispara.
    private void SpawnDummyProjectile(Vector3 position, Vector3 up)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        // Instanciar un proyectil de "disparo simulado" en el cliente para proporcionar 
        // retroalimentación visual inmediata al jugador que dispara.
        GameObject projectileInstance = Instantiate(clientProjectilePrefab, position, Quaternion.identity);
        // Configurar la dirección del proyectil para que apunte hacia donde está mirando el cañón del tanque
        projectileInstance.transform.up = up;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

    }

    [Rpc(SendTo.Server)]
    private void PrimaryFireServerRpc(Vector3 position, Vector3 up)
    {

        if (wallet.TotalCoins.Value < costToFire) { return; }

        wallet.SpendCoins(costToFire);

        GameObject projectileInstance = Instantiate(
            serverProjectilePrefab,
            position,
            Quaternion.identity);
        projectileInstance.transform.up = up;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
        // Si el proyectil tiene un componente DealDamageOnContact, 
        // se establece el propietario del daño para que el sistema de daño pueda atribuir correctamente el daño al jugador que disparó.
        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }
        // Configurar la velocidad del proyectil para que se mueva en la dirección que está mirando el cañón del tanque, 
        // multiplicando la velocidad de movimiento por la dirección hacia adelante del proyectil.
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(position, up);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnDummyProjectileClientRpc(Vector3 position, Vector3 up)
    {
        if (IsOwner) { return; }
        SpawnDummyProjectile(position, up);
    }
}
