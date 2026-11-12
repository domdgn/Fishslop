using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

namespace FishSlop
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : NetworkBehaviour
    {
        [SerializeField] private PlayerMovementHandler movement;
        [SerializeField] private PlayerLookHandler look;
        [SerializeField] private PlayerInteraction interaction;
        [SerializeField] private PlayerSteerShip steering;
        [SerializeField] private GameObject boop;

        private PlayerInput playerInput;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            if (look == null) look = GetComponent<PlayerLookHandler>();
            if (movement == null) movement = GetComponent<PlayerMovementHandler>();
            if (interaction == null) interaction = GetComponent<PlayerInteraction>();
            if (steering == null) steering = GetComponent<PlayerSteerShip>();
            if (boop != null) boop.SetActive(true);

            foreach (var actionMap in playerInput.actions.actionMaps)
            {
                actionMap.Disable();
            }

            playerInput.enabled = false;
        }

        public override void OnStartAuthority()
        {
            playerInput.enabled = true;

            if (look != null) look.EnableLook();
            if (movement != null) movement.enabled = true;
            if (steering != null) steering.enabled = true;
            if (interaction != null) interaction.enabled = true;
            if (boop != null) boop.SetActive(false);

            playerInput.actions.FindActionMap("Player").Enable();
            playerInput.SwitchCurrentActionMap("Player");
        }

        public override void OnStopAuthority()
        {
            playerInput.enabled = false;

            if (look != null) look.DisableLook();
        }

        public void OnMove(InputValue value)
        {
            if (!isOwned) return;
            movement.SetMoveInput(value.Get<Vector2>());
        }

        public void OnSprint(InputValue value)
        {
            if (!isOwned) return;
            movement.SetSprint(value.isPressed);
        }

        public void OnJump(InputValue value)
        {
            if (!isOwned || !value.isPressed) return;
            movement.Jump();
        }

        public void OnLook(InputValue value)
        {
            if (!isOwned || look == null) return;
            look.HandleLook(value.Get<Vector2>());
        }

        public void OnInteract(InputValue value)
        {
            if (!isOwned || !value.isPressed) return;
            Debug.Log("Use Input called");
            if (interaction != null) interaction.DoInteract();
        }
    }
}
