using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

namespace FishSlop
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovementHandler : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpForce = 6f;
        [SerializeField] private float jumpClampCooldown = 0.2f;
        private float lastJumpTime = -10f;

        [SerializeField] private float maxSlopeAngle = 50f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;

        [SerializeField] private float wallCheckDistance = 0.6f;
        [SerializeField] private float lowCheckHeight = 0.3f;
        [SerializeField] private float highCheckHeight = 1.0f;

        [SerializeField] private float flySpeed = 15f;
        public bool Flying { get; private set; }
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
        private Vector3 groundNormal = Vector3.up;
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

        private Vector3 BlockWalls(Vector3 vel)
        {
            Vector3 flat = new Vector3(vel.x, 0f, vel.z);
            if (flat.sqrMagnitude < 0.0001f) return vel;

            Vector3 dir = flat.normalized;

            vel = CheckWallAtHeight(vel, dir, lowCheckHeight);
            flat = new Vector3(vel.x, 0f, vel.z);
            if (flat.sqrMagnitude < 0.0001f) return vel;

            return CheckWallAtHeight(vel, flat.normalized, highCheckHeight);
        }

        private Vector3 CheckWallAtHeight(Vector3 vel, Vector3 dir, float height)
        {
            Vector3 origin = groundCheck.position + Vector3.up * height;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, wallCheckDistance,
                groundMask, QueryTriggerInteraction.Ignore))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > maxSlopeAngle)
                {
                    Vector3 wallNormal = new Vector3(hit.normal.x, 0f, hit.normal.z).normalized;
                    Vector3 flat = new Vector3(vel.x, 0f, vel.z);

                    if (Vector3.Dot(flat, wallNormal) < 0f)
                    {
                        flat = Vector3.ProjectOnPlane(flat, wallNormal);
                        vel.x = flat.x;
                        vel.z = flat.z;
                    }
                }
            }
            return vel;
        }

        private void FixedUpdate()
        {
            if (!isOwned) return;

            if (Flying)
            {
                FlyMove();
                return;
            }

            FindShip();

            bool grounded = IsGrounded();

            rb.MoveRotation(Quaternion.Euler(0f, Yaw, 0f));

            Vector3 dir = Quaternion.Euler(0f, Yaw, 0f) * new Vector3(moveInput.x, 0f, moveInput.y);
            if (grounded) dir = Vector3.ProjectOnPlane(dir, groundNormal).normalized * dir.magnitude;
            Vector3 velocity = dir * (isSprinting ? sprintSpeed : moveSpeed);

            velocity = BlockWalls(velocity);

            if(shipRb != null)
{
                Vector3 carry = ShipCarryVelocity();
                velocity.x += carry.x;
                velocity.z += carry.z;
            }

            bool justJumped = Time.time - lastJumpTime < jumpClampCooldown;

            float y = rb.linearVelocity.y;
            if (grounded && !justJumped && y > 0f)
            {
                y = 0f;
            }

            velocity.y = y;
            rb.linearVelocity = velocity;

            if (jumpPressed)
            {
                jumpPressed = false;
                if (IsGrounded())
                {
                    lastJumpTime = Time.time;
                    velocity.y = 0f;
                    rb.linearVelocity = velocity;
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            }
        }

        private Vector3 ShipCarryVelocity()
        {
            Vector3 p = rb.position;

            Vector3 sv = ship.GetSyncedPointVelocity(p);

            float omegaY = ship.syncedAngularVelocity.y; // rad/s
            if (Mathf.Abs(omegaY) < 0.0001f) return sv;

            float dt = Time.fixedDeltaTime;
            Vector3 pivot = ship.rotatePoint.position;
            Vector3 r = p - pivot;
            r.y = 0f;

            Vector3 tangent = Vector3.Cross(new Vector3(0f, omegaY, 0f), r);
            Vector3 arcDisp = Quaternion.AngleAxis(omegaY * Mathf.Rad2Deg * dt, Vector3.up) * r - r;

            return sv + (arcDisp / dt - tangent);
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
            Vector3 origin = groundCheck.position + Vector3.up * 0.1f;

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.3f, groundMask, QueryTriggerInteraction.Ignore))
            {
                groundNormal = hit.normal;
                return Vector3.Angle(hit.normal, Vector3.up) <= maxSlopeAngle;
            }

            groundNormal = Vector3.up;
            return false;
        }

        // DEBUG STUFFS
        public void SetFly(bool on)
        {
            Flying = on;
            rb.useGravity = !on;
            if (on) ClearMovementInput();
        }

        private void FlyMove()
        {
            rb.MoveRotation(Quaternion.Euler(0f, Yaw, 0f));

            Vector3 dir = Quaternion.Euler(0f, Yaw, 0f) * new Vector3(moveInput.x, 0f, moveInput.y);

            float vert = 0f;
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.spaceKey.isPressed) vert += 1f;
                if (kb.leftCtrlKey.isPressed) vert -= 1f;
            }

            float speed = isSprinting ? flySpeed * 2f : flySpeed;
            rb.linearVelocity = new Vector3(dir.x * speed, vert * speed, dir.z * speed);
            jumpPressed = false;
        }
    }
}