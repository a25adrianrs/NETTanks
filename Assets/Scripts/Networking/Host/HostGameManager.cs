using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private Allocation allocation;
    private string joinCode;

    private const int MaxConnections = 20;
    // En la constante GameSceneName se debe poner el nombre de la escena del juego, que en este caso es "Game by Adrián"
    // , pero si se cambia el nombre de la escena, se debe actualizar esta constante con el nuevo nombre
    private const string GameSceneName = "Game by Adrián";

    public async Task StartHostAsync()
    {
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "udp");
        transport.SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }
}