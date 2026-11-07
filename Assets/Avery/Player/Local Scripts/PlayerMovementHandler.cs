using UnityEngine;
using Mirror;

namespace FishSlop
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovementHandler : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpForce = 6f;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.2f;
        [SerializeField] private LayerMask groundMask;

        private Rigidbody rb;

        private Vector2 moveInput;
        private bool isSprinting;
        private bool jumpPressed;

        private float targetYaw;

        private ShipMovement currentShip;
        private Vector3 lastShipPosition;
        private Quaternion lastShipRotation;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            rb.isKinematic = true;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            rb.isKinematic = false;
            targetYaw = transform.eulerAngles.y;
        }

        [Command]
        public void CmdSetMoveInput(Vector2 input)
        {
            moveInput = Vector2.ClampMagnitude(input, 1f);
        }

        [Command]
        public void CmdSetSprint(bool sprinting)
        {
            isSprinting = sprinting;
        }

        [Command]
        public void CmdJump()
        {
            jumpPressed = true;
        }

        [Command]
        public void CmdSetYaw(float yaw)
        {
            targetYaw = yaw;
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            ApplyShipMovement();

            rb.MoveRotation(Quaternion.Euler(0f, targetYaw, 0f));

            HandleMovement();
            HandleJump();
        }

        [Server]
        private void HandleMovement()
        {
            Vector3 moveDirection = Quaternion.Euler(0f, targetYaw, 0f) * new Vector3(moveInput.x, 0f, moveInput.y);

            float speed = isSprinting ? sprintSpeed : moveSpeed;

            Vector3 velocity = moveDirection * speed;
            velocity.y = rb.linearVelocity.y;

            rb.linearVelocity = velocity;
        }

        [Server]
        private void HandleJump()
        {
            if (!jumpPressed) return;

            jumpPressed = false;

            if (!IsGrounded()) return;

            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        [Server]
        private void ApplyShipMovement()
        {
            if (currentShip == null) return;

            Transform ship = currentShip.transform;

            Vector3 localPosition = Quaternion.Inverse(lastShipRotation) * (rb.position - lastShipPosition);

            Vector3 newPosition = ship.position + ship.rotation * localPosition;

            Vector3 shipDelta = newPosition - rb.position;

            Quaternion rotationDelta = ship.rotation * Quaternion.Inverse(lastShipRotation);

            targetYaw = (rotationDelta * Quaternion.Euler(0f, targetYaw, 0f)).eulerAngles.y;

            rb.MovePosition(rb.position + shipDelta);

            lastShipPosition = ship.position;
            lastShipRotation = ship.rotation;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            ShipMovement ship = other.GetComponentInParent<ShipMovement>();

            if (ship == null) return;

            currentShip = ship;
            lastShipPosition = ship.transform.position;
            lastShipRotation = ship.transform.rotation;
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            ShipMovement ship = other.GetComponentInParent<ShipMovement>();

            if (ship == currentShip) currentShip = null;
        }

        [Server]
        public void ClearMovementInput()
        {
            moveInput = Vector2.zero;
            isSprinting = false;
            jumpPressed = false;
        }

        [Server]
        private bool IsGrounded()
        {
            return Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);
        }
    }
}
