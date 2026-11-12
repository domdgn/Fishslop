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

        private ShipMovement ship;
        private Rigidbody shipRb;
        private float lastOnShipTime;
        private Vector3 lastShipPosition;
        private Quaternion lastShipRotation;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void Update()
        {
            if (!isOwned || ship == null) return;
            Yaw += ship.syncedAngularVelocity.y * Mathf.Rad2Deg * Time.deltaTime;
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
                Vector3 sv = ship.GetSyncedPointVelocity(rb.position);
                //Debug.LogWarning($"Ship velocity: {sv} | Player velocity: {velocity}");
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
                var deck = hit.collider.GetComponent<ShipDeckFollower>();
                if (deck != null) 
                {
                    found = deck.shipRB;
                    ship = deck.shipMovement;
                }
            }

            if (found != null)
            {
                if (shipRb != found)
                {
                    shipRb = found;
                    lastShipPosition = shipRb.position;
                    lastShipRotation = shipRb.rotation;
                }

                lastOnShipTime = Time.time;
            }
            else if (Time.time - lastOnShipTime > 0.25f)
            {
                shipRb = null;
                ship = null;
            }
        }

        private bool IsGrounded()
        {
            return Physics.CheckSphere(groundCheck.position, groundRadius, groundMask, QueryTriggerInteraction.Ignore);
        }
    }
}