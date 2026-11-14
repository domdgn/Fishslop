using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using FishSlop;
using Mirror;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class DebugMenuSingleton : MonoBehaviour
{
    public static DebugMenuSingleton Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        AddPlayerButtons();
    }

    public void ToggleDebugMenu()
    {
        UDebugPanel.Toggle();
    }

    private void AddPlayerButtons()
    {
        IDebugActionsSection section = UDebugPanel.AddSection("Player");
        
        section.AddButton("Respawn", () =>
        {
            var player = NetworkClient.localPlayer;
            if (player == null)
            {
                Debug.LogWarning("No local player yet.");
                return;
            }

            var respawn = player.GetComponent<PlayerRespawn>();
            if (respawn != null) respawn.CmdRequestRespawn();
        });

        section.AddButton("Toggle Fly", () =>
        {
            var player = NetworkClient.localPlayer;
            if (player == null) return;

            var move = player.GetComponent<PlayerMovementHandler>();
            if (move != null) move.SetFly(!move.Flying);
        });
    }
}
