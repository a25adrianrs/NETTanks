using Unity.Netcode;
using UnityEngine;

/// <summary>
<<<<<<< HEAD
/// Este script gestiona la conexión del cliente a un servidor de juego multiplayer.
/// Se encarga de inicializar la conexión de red utilizando el NetworkManager de Netcode for GameObjects.
=======
/// Inicia la conexión del cliente al servidor usando el NetworkManager global.
/// Se puede enlazar a un botón de menú para unir al jugador a la partida.
>>>>>>> 7744943846ddb7baf55f522dd160659aa7c42d59
/// </summary>
public class JoinServer : MonoBehaviour
{
    /// <summary>
    /// Se ejecuta al hacer click en el botón para conectarse al servidor.
    /// Inicia el cliente de red para que se conecte a un servidor existente.
    /// </summary>
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
}
