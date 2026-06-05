using Unity.Netcode;
using Unity.Cinemachine;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera followCam;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            followCam.Priority = 10;
        }
        else
        {
            followCam.Priority = 0;
        }
    }
}