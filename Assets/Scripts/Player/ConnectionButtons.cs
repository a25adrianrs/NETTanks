using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Botones de UI para iniciar la partida como host o cliente.
/// Usa NetworkManager.Singleton para iniciar la sesión de red.
/// </summary>
public class ConnectionButtons : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
