using Unity.Netcode;
using UnityEngine;

/// <summary>
<<<<<<< HEAD
/// Este script gestiona los botones de conexión de red para el modo multiplayer.
/// Proporciona métodos para que el servidor (host) se inicie o para que los clientes se conecten.
/// Se conecta a los botones de la UI para controlar la red.
=======
/// Botones de UI para iniciar la partida como host o cliente.
/// Usa NetworkManager.Singleton para iniciar la sesión de red.
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
/// </summary>
public class ConnectionButtons : MonoBehaviour
{
    /// <summary>
    /// Se ejecuta al hacer click en el botón "Host" o "Servidor".
    /// Inicia el juego como servidor que espera conexiones de otros jugadores.
    /// El host actúa como servidor autorizado para validar acciones de los jugadores.
    /// </summary>
    public void StartHost()
    {
        // Inicia el NetworkManager como host (servidor)
        // El host es responsable de autorizar acciones y sincronizar el estado del juego con todos los clientes
        NetworkManager.Singleton.StartHost();
    }

<<<<<<< HEAD
    /// <summary>
    /// Se ejecuta al hacer click en el botón "Cliente" o "Conectar".
    /// Inicia el juego como cliente que se conecta a un servidor existente.
    /// El cliente envía inputs pero el servidor valida las acciones.
    /// </summary>
=======
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
    public void StartClient()
    {
        // Inicia el NetworkManager como cliente
        // El cliente se conectará al servidor que espera conexiones (el host)
        NetworkManager.Singleton.StartClient();
    }
}
