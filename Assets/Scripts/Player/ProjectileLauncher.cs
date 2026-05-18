using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Este script gestiona el disparo de proyectiles en multiplicador.
/// Crea un disparo visual inmediato en el cliente para feedback rápido y un proyectil real en el servidor
/// que sincroniza con todos los jugadores. Esto asegura que todos vean el disparo al mismo tiempo.
/// También maneja el sistema de costos (monedas) para disparar y evita daño amistoso.
/// </summary>
public class ProjectileLauncher : NetworkBehaviour
{
        /// <summary>
        /// Referencia al componente InputReader que proporciona las entradas del jugador.
        /// Se utiliza para detectar cuándo el jugador presiona el botón de disparo.
        /// </summary>
        [SerializeField] private InputReader inputReader;

        /// <summary>
        /// Referencia al Transform del punto desde donde se lanzan los proyectiles.
        /// Típicamente está en la punta del cañón para que los proyectiles salgan de allí.
        /// </summary>
        [SerializeField] private Transform projectileSpawnPoint;

        /// <summary>
        /// Prefab del proyectil real que se spawned en el servidor.
        /// Este proyectil causa daño y se sincroniza con todos los jugadores.
        /// Solo el servidor puede instanciar este prefab.
        /// </summary>
        [SerializeField] private GameObject serverProjectilePrefab;

        /// <summary>
        /// Prefab del proyectil visual que aparece inmediatamente en el cliente.
        /// Este proyectil es solo visual y no causa daño realmente.
        /// Se usa para dar feedback rápido al jugador que dispara (sin esperar al servidor).
        /// </summary>
        [SerializeField] private GameObject clientProjectilePrefab;

        /// <summary>
        /// Referencia al objeto visual del fogonazo del cañón (muzzle flash).
        /// Se activa cuando se dispara para crear un efecto visual de fuego.
        /// </summary>
        [SerializeField] private GameObject muzzleFlash;

        /// <summary>
        /// Referencia al Collider2D del jugador.
        /// Se usa para ignorar colisiones entre el jugador y sus propios proyectiles.
        /// Sin esto, el jugador se dañaría a sí mismo al disparar hacia delante.
        /// </summary>
        [SerializeField] private Collider2D playerCollider;

        /// <summary>
        /// Referencia al script CoinWallet del jugador.
        /// Se utiliza para verificar si el jugador tiene suficientes monedas para disparar
        /// y para gastar monedas cuando se dispara.
        /// </summary>
        [SerializeField] private CoinWallet wallet;

        /// <summary>
        /// Velocidad a la que viajan los proyectiles después de ser lanzados.
        /// Valores más altos hacen proyectiles más rápidos.
        /// </summary>
        [SerializeField] private float projectileSpeed;

        /// <summary>
        /// Tasa de fuego en disparos por segundo.
        /// Por ejemplo, si fireRate = 2, el jugador puede disparar 2 veces por segundo.
        /// Se convierte a tiempo de espera entre disparos en Update.
        /// </summary>
        [SerializeField] private float fireRate;

        /// <summary>
        /// Duración en segundos que el fogonazo del cañón permanece visible.
        /// Valores más bajos crean un efecto más rápido, más altos un efecto más dramático.
        /// </summary>
        [SerializeField] private float muzzleFlashDuration;

        /// <summary>
        /// Costo en monedas de disparar una vez.
        /// Se resta del CoinWallet del jugador cada vez que dispara.
        /// Permite balancear el juego limitando la cantidad de disparos.
        /// </summary>
        [SerializeField] private int costToFire;

        /// <summary>
        /// Bandera que indica si el jugador está presionando el botón de disparo.
        /// Se actualiza en HandlePrimaryFire y se verifica en Update.
        /// </summary>
        private bool shouldFire;

        /// <summary>
        /// Temporizador que controla la tasa de fuego (cadencia de disparo).
        /// Cuando es > 0, el jugador no puede disparar aún.
        /// Se reduce en cada Update hasta llegar a 0, momento en que se permite disparar de nuevo.
        /// </summary>
        private float timer;

        /// <summary>
        /// Temporizador del efecto visual del fogonazo del cañón.
        /// Cuando es > 0, el fogonazo está visible.
        /// Se reduce en cada Update y se desactiva cuando llega a 0.
        /// </summary>
        private float muzzleFlashTimer;

        /// <summary>
        /// Se ejecuta cuando el objeto aparece en la red.
        /// Suscribimos al evento de disparo solo si somos el dueño de este tanque.
        /// </summary>
        public override void OnNetworkSpawn()
        {
                // Solo el dueño puede disparar
                if (!IsOwner) { return; }

                // Suscribirse al evento de disparo del InputReader
                // HandlePrimaryFire se ejecutará cada vez que se presione/suelte el botón de disparo
                inputReader.PrimaryFireEvent += HandlePrimaryFire;
        }

        /// <summary>
        /// Se ejecuta cuando el objeto desaparece de la red.
        /// Desuscribimos del evento de disparo para evitar errores.
        /// </summary>
        public override void OnNetworkDespawn()
        {
                // Solo el dueño puede disparar
                if (!IsOwner) { return; }

                // Desuscribirse del evento de disparo
                inputReader.PrimaryFireEvent -= HandlePrimaryFire;
        }

        /// <summary>
        /// Se ejecuta cuando el InputReader detecta que el botón de disparo fue presionado o soltado.
        /// Simplemente actualiza la bandera shouldFire.
        /// El verdadero manejo del disparo ocurre en Update.
        /// </summary>
        private void HandlePrimaryFire(bool shouldFire)
        {
                // Guarda si el jugador quiere disparar (true = presionando botón, false = soltó el botón)
                this.shouldFire = shouldFire;
        }

        /// <summary>
        /// Se ejecuta cada frame.
        /// Controla:
        /// - El temporizador del fogonazo (efecto visual del disparo)
        /// - El temporizador de cadencia de disparo
        /// - El procesamiento del disparo si todas las condiciones se cumplen
        /// </summary>
        void Update()
        {
                // Manejar el temporizador del fogonazo del cañón
                // Si el fogonazo aún está activo (timer > 0), reducirlo
                if (muzzleFlashTimer > 0f)
                {
                        muzzleFlashTimer -= Time.deltaTime;

                        // Si el temporizador del fogonazo ha expirado (≤ 0), desactivar el objeto del fogonazo
                        // para que desaparezca visualmente
                        if (muzzleFlashTimer <= 0f)
                        {
                                // Desactiva el GameObject del fogonazo para que no sea visible
                                muzzleFlash.SetActive(false);
                        }
                }

                // Solo el dueño de este tanque puede disparar
                if (!IsOwner) { return; }

                // Manejar el temporizador de cadencia de disparo
                // Si el temporizador es > 0, significa que aún hay que esperar antes de poder disparar de nuevo
                if (timer > 0)
                {
                        // Reduce el temporizador basado en el tiempo que ha pasado
                        timer -= Time.deltaTime;
                }

                // Si el jugador no está intentando disparar, salir del método
                if (!shouldFire) { return; }

                // Verificar si es demasiado pronto para disparar de nuevo (tasa de fuego)
                // Si el timer aún está contando hacia abajo, no permitir otro disparo
                if (timer > 0) { return; }

                // Verificar si el jugador tiene suficientes monedas para disparar
                // Si no tiene suficientes, no hacer nada
                if (wallet.TotalCoins.Value < costToFire) { return; }

                // Llamar al método RPC para disparar en el servidor
                // Se envía la posición y dirección del cañón para que el servidor spawnee el proyectil correcto
                PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

                // Llamar al método para crear un proyectil visual en el cliente
                // Proporciona feedback visual inmediato sin esperar al servidor
                SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

                // Reiniciar el temporizador de disparo para controlar la tasa de fuego
                // 1 / fireRate convierte disparos por segundo a segundos entre disparos
                // Por ejemplo, si fireRate = 2, timer = 1/2 = 0.5 segundos entre disparos
                timer = 1 / fireRate;
        }

        /// <summary>
        /// Crea un proyectil visual local para feedback inmediato del jugador.
        /// Este proyectil no causa daño realmente, solo se ve.
        /// Se spawnea inmediatamente sin esperar al servidor.
        /// </summary>
        private void SpawnDummyProjectile(Vector3 position, Vector3 up)
        {
                // Activa el fogonazo del cañón para mostrar el efecto de disparo
                muzzleFlash.SetActive(true);

                // Reinicia el temporizador del fogonazo para que brille durante la duración especificada
                muzzleFlashTimer = muzzleFlashDuration;

                // Instancia un proyectil visual local en el cliente
                // Este proyectil es solo para ver, no causa daño ni se sincroniza con otros jugadores
                GameObject projectileInstance = Instantiate(clientProjectilePrefab, position, Quaternion.identity);

                // Configura la dirección del proyectil para que apunte hacia donde apunta el cañón
                // transform.up convierte el vector dirección en la orientación del objeto
                projectileInstance.transform.up = up;

                // Ignora colisiones entre el jugador y su propio proyectil
                // Evita que el proyectil golpee al jugador que lo disparó
                Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

                // Si el proyectil tiene un Rigidbody2D, se le asigna velocidad
                if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                {
                        // Establece la velocidad del proyectil para que se mueva en la dirección del cañón
                        rb.linearVelocity = rb.transform.up * projectileSpeed;
                }
        }

        /// <summary>
        /// RPC que se ejecuta en el servidor para crear el proyectil real.
        /// El servidor es responsable de hacer el projectil "oficial" que causa daño.
        /// Los parámetros se sincronizan desde el cliente al servidor.
        /// </summary>
        [Rpc(SendTo.Server)]
        private void PrimaryFireServerRpc(Vector3 position, Vector3 up)
        {
                // Verificar si el servidor tiene suficientes monedas del jugador (si es que se cambió en el servidor)
                // Esto es una verificación adicional en el servidor para evitar trucos
                if (wallet.TotalCoins.Value < costToFire) { return; }

                // Restar el costo del disparo de las monedas del jugador
                wallet.SpendCoins(costToFire);

                // Instancia el proyectil real que será sincronizado con todos los jugadores
                GameObject projectileInstance = Instantiate(
                    serverProjectilePrefab,
                    position,
                    Quaternion.identity);

                // Configura la dirección del proyectil para que apunte hacia donde apunta el cañón
                projectileInstance.transform.up = up;

                // Ignora colisiones entre el jugador que disparó y su propio proyectil
                Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

                // Si el proyectil tiene un componente DealDamageOnContact,
                // se establece el propietario del daño para que el sistema sepa quién disparó
                if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
                {
                        // OwnerClientId es el ID de la red del jugador que posee este tanque
                        dealDamage.SetOwner(OwnerClientId);
                }

                // Configurar la velocidad del proyectil para que se mueva hacia la dirección del cañón
                if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                {
                        // Multiplica la dirección (transform.up) por la velocidad para obtener la velocidad final
                        rb.linearVelocity = rb.transform.up * projectileSpeed;
                }

                // Spawnea el proyectil en la red para que todos los clientes lo vean
                // Esto es automático cuando se instancia un GameObject con NetworkObject
                // Luego, notifica a todos los clientes para que creen el proyectil visual adicional
                SpawnDummyProjectileClientRpc(position, up);
        }

        /// <summary>
        /// RPC que se ejecuta en todos los clientes (excepto el dueño del tanque).
        /// Crea un proyectil visual adicional en los clientes para sincronizar lo que ven.
        /// Los clientes ya tienen el proyectil visual del dueño, pero necesitan verlo desde su perspectiva.
        /// </summary>
        [Rpc(SendTo.ClientsAndHost)]
        private void SpawnDummyProjectileClientRpc(Vector3 position, Vector3 up)
        {
                // Si este cliente es el dueño del tanque que disparó, ignora esta RPC
                // Ya creó su propio proyectil visual en SpawnDummyProjectile, así que no necesita crear otro
                if (IsOwner) { return; }

                // Para otros clientes, crea el proyectil visual usando el mismo método
                SpawnDummyProjectile(position, up);
        }
}
