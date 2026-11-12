using UnityEngine.InputSystem;
using UnityEngine;
using Mirror;

namespace FishSlop
{
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
            if (ship == null) return;

            NetworkIdentity shipIdentity = ship.GetComponentInParent<NetworkIdentity>();
            if (shipIdentity == null)
            {
                Debug.LogError("ShipMovement does not have a NetworkIdentity component in its parent.");
                return;
            }

            if (isOwned)
            {
                BeginSteeringLocal(shipIdentity.netId);
            }
            else if (isServer)
            {
                TargetBeginSteering(shipIdentity.netId);
            }
        }

        [TargetRpc]
        private void TargetBeginSteering(uint netId)
        {
            BeginSteeringLocal(netId);
        }

        private void BeginSteeringLocal(uint netId)
        {
            shipNetId = netId;
            steerInput = 0f;

            GetComponent<PlayerMovementHandler>().ClearMovementInput();
            playerInput.SwitchCurrentActionMap("Ship");
        }

        public void OnStopSteering(InputValue value)
        {
            Debug.Log($"StopSteering callback | Pressed: {value.isPressed} | Map: {playerInput.currentActionMap.name}");

            if (!value.isPressed || !isOwned) return;
            if (playerInput.currentActionMap == null || playerInput.currentActionMap.name != "Ship") return;

            CmdSetSteerInput(shipNetId, 0f);

            shipNetId = 0;
            steerInput = 0f;

            playerInput.SwitchCurrentActionMap("Player");
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
            Debug.Log($"[PlayerSteerShip] Server received steer command. Ship ID: {targetShipId}, Direction: {direction}");

            if (!NetworkServer.spawned.TryGetValue(targetShipId, out NetworkIdentity identity))
            {
                Debug.LogError($"[PlayerSteerShip] Server cannot find ship with netId {targetShipId}");
                return;
            }

            ShipMovement ship = identity.GetComponentInChildren<ShipMovement>(true);

            if (ship == null)
            {
                Debug.LogError("[PlayerSteerShip] Server found NetworkIdentity but no ShipMovement.");
                return;
            }

            ship.SetSteerInput(direction);
        }
    }
}