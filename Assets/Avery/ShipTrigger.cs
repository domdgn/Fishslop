using FishSlop;
using Mirror;
using UnityEngine;

public class ShipTrigger : NetworkBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (!other.TryGetComponent(out ShipMovement shipMovement)) return;

        NetworkManager.singleton.ServerChangeScene("WinScene");
    }
}
