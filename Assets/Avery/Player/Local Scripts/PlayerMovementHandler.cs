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
        private Rigidbody shipRb;
        private int shipOverlaps;

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

        [Command]
        public void CmdAddYaw(float delta)
        {
            targetYaw += delta;
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            if (shipRb != null) targetYaw += shipRb.angularVelocity.y * Mathf.Rad2Deg * Time.fixedDeltaTime;

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

            if (shipRb != null)
            {
                Vector3 shipVel = shipRb.GetPointVelocity(rb.position);
                velocity.x += shipVel.x;
                velocity.z += shipVel.z;
            }

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

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            ShipMovement ship = other.GetComponentInParent<ShipMovement>();
            if (ship == null) return;

            if (ship != currentShip)
            {
                currentShip = ship;
                shipRb = ship.GetComponent<Rigidbody>();
                shipOverlaps = 0;
            }
            shipOverlaps++;
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            ShipMovement ship = other.GetComponentInParent<ShipMovement>();
            if (ship == null || ship != currentShip) return;

            shipOverlaps--;

            if (shipOverlaps <= 0)
            {
                currentShip = null;
                shipRb = null;
                shipOverlaps = 0;
            }
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
