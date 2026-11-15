using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using FishSlop;
using Mirror;
using Pinwheel.Griffin.PoseidonPreview;
using UnityEngine;

public class DebugMenuSingleton : MonoBehaviour
{
    public static DebugMenuSingleton Instance { get; private set; }
    [SerializeField] private GameObject plankPrefab;
    private bool tinyMode;
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
        AddShipButtons();
        AddOtherButtons();
    }

    public void ToggleDebugMenu()
    {
        UDebugPanel.Toggle();
    }

    private void AddPlayerButtons()
    {
        IDebugActionsSection section = UDebugPanel.AddSection("Player");
        
        section.AddButton("Teleport To Ship", () =>
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

    private void AddShipButtons()
    {
        IDebugActionsSection section = UDebugPanel.AddSection("Ship");

        section.AddButton("Reset Ship Health", () =>
        {
            if (!NetworkServer.active) return;

            foreach (var identity in NetworkServer.spawned.Values)
            {
                var ship = identity.GetComponent<ShipMovement>();
                if (ship == null) continue;

                ship.ResetShip();
                break;
            }
        });
    }

    private void AddOtherButtons()
    {
        IDebugActionsSection section = UDebugPanel.AddSection("Other");

        section.AddButton("Spawn Plank", () =>
        {
            if (!NetworkServer.active) return;

            var player = NetworkClient.localPlayer;
            if (player == null) return;

            Transform spawnPoint = player.transform;

            Vector3 position = spawnPoint.position + spawnPoint.forward * 2f;
            Quaternion rotation = Quaternion.identity;

            GameObject plank = Instantiate(plankPrefab, position, rotation);
            NetworkServer.Spawn(plank);
        });

        section.AddButton("Launch Ship Upwards", () =>
        {
            if (!NetworkServer.active) return;

            foreach (var identity in NetworkServer.spawned.Values)
            {
                var ship = identity.GetComponent<ShipMovement>();
                if (ship == null) continue;

                var rb = ship.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.up * 50000f, ForceMode.Impulse);
                break;
            }
        });

        section.AddButton("Toggle Tiny Mode", () =>
        {
            var player = NetworkClient.localPlayer;
            if (player == null) return;

            tinyMode = !tinyMode;
            player.transform.localScale = tinyMode ? Vector3.one * 0.3f : Vector3.one;
        });

        section.AddButton("Bring Everyone to Host", () =>
        {
            if (!NetworkServer.active) return;

            var host = NetworkClient.localPlayer;
            if (host == null) return;

            Vector3 hostPosition = host.transform.position;
            int index = 0;

            foreach (var connection in NetworkServer.connections.Values)
            {
                var player = connection.identity;
                if (player == null || player == host) continue;

                Vector3 offset = Vector3.forward * (index * 2f);

                var movement = player.GetComponent<PlayerMovementHandler>();

                movement?.TargetTeleport(connection, hostPosition + offset);

                index++;
            }
        });
    }
}
