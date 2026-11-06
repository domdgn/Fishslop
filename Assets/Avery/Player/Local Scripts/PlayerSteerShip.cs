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
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    public void StartSteeringShip(ShipMovement ship)
    {
        if (!isOwned || ship == null) return;

        shipMovement = ship;
        GetComponent<PlayerMovementHandler>().ClearMovementInput();
        playerInput.SwitchCurrentActionMap("Ship");
    }

    public void OnStopSteering(InputValue value)
    {
        Debug.Log($"StopSteering callback | Pressed: {value.isPressed} | Map: {playerInput.currentActionMap.name}");

        if (!value.isPressed) return;
        if (playerInput.currentActionMap.name != "Ship") return;

        shipMovement = null;
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
    }
}
