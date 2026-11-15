using Mirror;
using Mirror.Examples.Benchmark;
using Unity.VisualScripting;
using UnityEngine;

namespace FishSlop
{
    public class ShipMovement : NetworkBehaviour
    {
        public float topSpeed = 10f;
        public float acceleration = 2f;
        public float deceleration = 1f;
        public float steerSpeed = 20f;

        private Rigidbody rb;
        private SailHandler sailHandler;
        private float steerInput;
        public Transform rotatePoint;
        public Transform wheel;

        private ShipHealth shipHealth;

        [SyncVar]
        public Vector3 syncedVelocity;

        [SyncVar]
        public Vector3 syncedAngularVelocity;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            shipHealth = GetComponent<ShipHealth>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.useGravity = true;
            rb.angularDamping = 2f;
            ShipWheel shipWheel = GetComponentInChildren<ShipWheel>(true);
            if (shipWheel != null)
            {
                rotatePoint = shipWheel.transform;
            }
        }

        private void Start()
        {
            sailHandler = GetComponentInChildren<SailHandler>(true);
        }

        public override void OnStartServer()
        {
            rb.isKinematic = false;
        }

        public override void OnStartClient()
        {
            if (!isServer) rb.isKinematic = true;
        }

        public Vector3 GetSyncedPointVelocity(Vector3 worldPosition)
        {
            Vector3 offset = worldPosition - transform.position;

            return syncedVelocity + Vector3.Cross(syncedAngularVelocity, offset);
        }

        [Server]
        public void SetSteerInput(float direction)
        {
            steerInput = Mathf.Clamp(direction, -1f, 1f);
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            if (sailHandler == null) return;

            Vector3 forward = -transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (sailHandler.isOpen)
            {
                rb.AddForce(forward * acceleration, ForceMode.Acceleration);
            }
            else
            {
                Vector3 slowed = Vector3.MoveTowards(flatVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
                rb.linearVelocity = new Vector3(slowed.x, rb.linearVelocity.y, slowed.z);
            }

            float forwardSpeed = Vector3.Dot(flatVelocity, forward);
            if (forwardSpeed > topSpeed) rb.linearVelocity -= forward * (forwardSpeed - topSpeed);

            float speedFactor = Mathf.Clamp01(flatVelocity.magnitude / topSpeed);
            float angle = steerInput * steerSpeed * speedFactor * Time.fixedDeltaTime;

            rb.AddTorque(Vector3.up * steerInput * steerSpeed * speedFactor, ForceMode.Acceleration);

            syncedVelocity = rb.linearVelocity;
            syncedAngularVelocity = rb.angularVelocity;

            if (wheel != null) // doesnt turn like a reeal ship yet with the locking or whatever but its okay
            {
                wheel.Rotate(Vector3.forward, steerInput * 100f * Time.fixedDeltaTime, Space.Self);
            }
        }

        [ServerCallback]
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponentInParent<PlayerMovement>())
            {
                return;
            }

            float impactSpeed = collision.relativeVelocity.magnitude;
            float damageTaken = impactSpeed * shipHealth.impactMulti;
            shipHealth.TakeDamage(damageTaken);

            Debug.Log($"Ship crashed into {collision.gameObject.name} / Impact speed: {impactSpeed} / Damage: {damageTaken}");
        }
    }
}