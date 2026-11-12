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
        public float Yaw { get; private set; }

        private Rigidbody shipRb;
        private float lastOnShipTime;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void Update()
        {
            if (!isOwned || shipRb == null) return;
            Yaw += shipRb.angularVelocity.y * Mathf.Rad2Deg * Time.deltaTime;
        }

        public override void OnStartAuthority()
        {
            rb.isKinematic = false;
            Yaw = transform.eulerAngles.y;
        }

        public override void OnStopAuthority()
        {
            rb.isKinematic = true;
        }

        public void SetMoveInput(Vector2 input) => moveInput = Vector2.ClampMagnitude(input, 1f);
        public void SetSprint(bool sprinting) => isSprinting = sprinting;
        public void Jump() => jumpPressed = true;
        public void AddYaw(float delta) => Yaw += delta;

        public void ClearMovementInput()
        {
            moveInput = Vector2.zero;
            isSprinting = false;
            jumpPressed = false;
        }

        [TargetRpc]
        public void TargetClearMovementInput(NetworkConnectionToClient conn) => ClearMovementInput();

        private void FixedUpdate()
        {
            if (!isOwned) return;

            FindShip();

            rb.MoveRotation(Quaternion.Euler(0f, Yaw, 0f));

            Vector3 dir = Quaternion.Euler(0f, Yaw, 0f) * new Vector3(moveInput.x, 0f, moveInput.y);
            Vector3 velocity = dir * (isSprinting ? sprintSpeed : moveSpeed);

            if (shipRb != null)
            {
                Vector3 sv = shipRb.GetPointVelocity(rb.position);
                velocity.x += sv.x;
                velocity.z += sv.z;
            }

            velocity.y = rb.linearVelocity.y;
            rb.linearVelocity = velocity;

            if (jumpPressed)
            {
                jumpPressed = false;
                if (IsGrounded())
                {
                    velocity.y = 0f;
                    rb.linearVelocity = velocity;
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            }
        }

        private void FindShip()
        {
            Rigidbody found = null;

            if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, 2f, groundMask, QueryTriggerInteraction.Ignore))
            {
                ShipMovement s = hit.collider.GetComponentInParent<ShipMovement>();
                if (s != null) found = s.GetComponent<Rigidbody>();
            }

            if (found != null)
            {
                shipRb = found;
                lastOnShipTime = Time.time;
            }
            else if (Time.time - lastOnShipTime > 0.25f)
            {
                shipRb = null;
            }
        }

        private bool IsGrounded()
        {
            return Physics.CheckSphere(groundCheck.position, groundRadius, groundMask, QueryTriggerInteraction.Ignore);
        }
    }
}