using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Gestiona el disparo del jugador en red.
/// El cliente crea un proyectil visual inmediato y el servidor genera el proyectil real en la partida.
/// </summary>
public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    // Entrada del jugador para disparar
    [SerializeField] private InputReader inputReader;
    // Punto desde donde se genera el proyectil
    [SerializeField] private Transform projectileSpawnPoint;
    // Prefab usado por el servidor para la bala real
    [SerializeField] private GameObject serverProjectilePrefab;
    // Prefab usado por el cliente para el proyectil visual inmediato
    [SerializeField] private GameObject clientProjectilePrefab;
    // Efecto visual de fogonazo al disparar
    [SerializeField] private GameObject muzzleFlash;
    // Collider del jugador para evitar que el proyectil choque con él
    [SerializeField] private Collider2D playerCollider;
    // Monedas que consume el jugador para disparar
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

    private void Update()
    {
        // Controla la duración del fogonazo en pantalla
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) { return; }
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }

        if (!shouldFire) { return; }
        if (timer > 0f) { return; }
        if (wallet.TotalCoins.Value < costToFire) { return; }

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        timer = 1f / fireRate;
    }

    // Crea un proyectil visual en el cliente para dar feedback inmediato al jugador.
    private void SpawnDummyProjectile(Vector3 position, Vector3 up)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(clientProjectilePrefab, position, Quaternion.identity);
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

        GameObject projectileInstance = Instantiate(serverProjectilePrefab, position, Quaternion.identity);
        projectileInstance.transform.up = up;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }

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
