using Unity.Netcode;
using UnityEngine;

public class JoinServer : MonoBehaviour
{
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
