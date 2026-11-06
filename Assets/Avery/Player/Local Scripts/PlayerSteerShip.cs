using UnityEngine.InputSystem;
using UnityEngine;
using Mirror;
using static UnityEngine.Rendering.DebugUI;
using FishSlop;
using Steamworks;
public class PlayerSteerShip : NetworkBehaviour
{
    private ShipMovement shipMovement;
    private PlayerInput playerInput;
    private float steerInput;

    private uint shipNetId;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    public void StartSteeringShip(ShipMovement ship)
    {
        if (!isOwned || ship == null) return;

        NetworkIdentity shipIdentity = ship.GetComponentInParent<NetworkIdentity>();

        if (shipIdentity == null)
        {
            Debug.LogError("ShipMovement does not have a NetworkIdentity component in its parent.");
            return;
        }

        shipNetId = shipIdentity.netId;
        shipMovement = ship;
        GetComponent<PlayerMovementHandler>().ClearMovementInput();
        playerInput.SwitchCurrentActionMap("Ship");
    }

    public void OnStopSteering(InputValue value)
    {
        Debug.Log($"StopSteering callback | Pressed: {value.isPressed} | Map: {playerInput.currentActionMap.name}");

        if (!value.isPressed) return;
        if (playerInput.currentActionMap.name != "Ship") return;

        CmdSetSteerInput(shipNetId, 0f);

        shipMovement = null;
        shipNetId = 0;
        playerInput.SwitchCurrentActionMap("Player");
    }

    private void Update()
    {
        if (!isOwned || shipMovement == null) return;

        shipMovement.Steer(steerInput);
    }

    public void OnSteer(InputValue value)
    {
        if (!isOwned || shipMovement == null) return;

        steerInput = value.Get<Vector2>().x;

        CmdSetSteerInput(shipNetId, steerInput);
    }

    [Command]
    private void CmdSetSteerInput(uint targetShipId, float direction)
    {
        if (!NetworkServer.spawned.TryGetValue(targetShipId, out NetworkIdentity identity))
            return;

        ShipMovement ship = identity.GetComponentInChildren<ShipMovement>(true);

        if (ship != null)
            ship.SetSteerInput(direction);
    }
}