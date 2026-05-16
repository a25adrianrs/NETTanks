using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Este script gestiona la conexión del cliente a un servidor de juego multiplayer.
/// Se encarga de inicializar la conexión de red utilizando el NetworkManager de Netcode for GameObjects.
/// </summary>
public class JoinServer : MonoBehaviour
{
    /// <summary>
    /// Se ejecuta al hacer click en el botón para conectarse al servidor.
    /// Inicia el cliente de red para que se conecte a un servidor existente.
    /// </summary>
    public void Join()
    {
        // Mediante el NetworkManager, iniciamos el cliente para conectarnos al servidor.
        // Esto permitirá que el cliente se conecte al servidor y comience a recibir datos de la red.
        // Singelton es una propiedad estática que proporciona acceso 
        // a la instancia única del NetworkManager en la escena, se usa principalmente 
        // para acceder a la instancia del NetworkManager desde cualquier lugar del código.
        NetworkManager.Singleton.StartClient();
    }
}
