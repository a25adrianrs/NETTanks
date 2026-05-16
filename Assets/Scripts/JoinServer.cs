using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Inicia la conexión del cliente al servidor usando el NetworkManager global.
/// Se puede enlazar a un botón de menú para unir al jugador a la partida.
/// </summary>
public class JoinServer : MonoBehaviour
{
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
}
