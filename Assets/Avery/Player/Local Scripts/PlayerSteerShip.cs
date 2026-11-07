using UnityEngine.InputSystem;
using UnityEngine;
using Mirror;
using FishSlop;
public class PlayerSteerShip : NetworkBehaviour
{
    private PlayerInput playerInput;
    private float steerInput;
    private uint shipNetId;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        enabled = false;
    }
    public override void OnStartAuthority()
    {
        enabled = true;
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
        steerInput = 0f;

        GetComponent<PlayerMovementHandler>().ClearMovementInput();

        playerInput.actions.FindActionMap("Player").Disable();
        playerInput.actions.FindActionMap("Ship").Enable();
    }

    public void OnStopSteering(InputValue value)
    {
        Debug.Log($"StopSteering callback | Pressed: {value.isPressed} | Map: {playerInput.currentActionMap.name}");

        if (!value.isPressed || !isOwned) return;
        if (playerInput.currentActionMap == null || playerInput.currentActionMap.name != "Ship") return;

        CmdSetSteerInput(shipNetId, 0f);

        shipNetId = 0;
        steerInput = 0f;

        playerInput.actions.FindActionMap("Player").Enable();
        playerInput.actions.FindActionMap("Ship").Disable();
    }
    public void OnSteer(InputValue value)
    {
        if (!isOwned || shipNetId == 0) return;

        steerInput = value.Get<Vector2>().x;

        CmdSetSteerInput(shipNetId, steerInput);
    }

    [Command]
    private void CmdSetSteerInput(uint targetShipId, float direction)
    {
        if (!NetworkServer.spawned.TryGetValue(targetShipId, out NetworkIdentity identity)) return;

        ShipMovement ship = identity.GetComponentInChildren<ShipMovement>(true);

        if (ship != null) ship.SetSteerInput(direction);
    }
}