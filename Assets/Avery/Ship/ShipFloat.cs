using Mirror;
using UnityEngine;

namespace FishSlop
{
    [RequireComponent(typeof(Rigidbody))]
    public class ObjectFloat : NetworkBehaviour
    {
        [SerializeField] private Collider hullCollider;
        [SerializeField] private float restDepth = 0.5f;
        [SerializeField] private float damping = 0.4f;
        [SerializeField] private bool simulateOnClients = false;
        [SerializeField] private float horizontalDrag = 2f;

        private Rigidbody rb;
        private Grabbable grabbable;
        private Vector3[] floatPoints = new Vector3[4];
        private float strength;
        private float dampingForce;
        private bool ready;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            grabbable = GetComponent<Grabbable>();
            if (hullCollider == null) hullCollider = GetComponent<Collider>();
        }

        private void InitialiseBuoyancy()
        {
            Physics.SyncTransforms();

            Bounds b = hullCollider.bounds;
            floatPoints[0] = transform.InverseTransformPoint(new Vector3(b.min.x, b.min.y, b.min.z));
            floatPoints[1] = transform.InverseTransformPoint(new Vector3(b.max.x, b.min.y, b.min.z));
            floatPoints[2] = transform.InverseTransformPoint(new Vector3(b.min.x, b.min.y, b.max.z));
            floatPoints[3] = transform.InverseTransformPoint(new Vector3(b.max.x, b.min.y, b.max.z));

            strength = rb.mass * Physics.gravity.magnitude / (floatPoints.Length * restDepth);
            dampingForce = damping * 2f * Mathf.Sqrt(strength * rb.mass / floatPoints.Length);

            ready = true;
        }

        public override void OnStartServer()
        {
            InitialiseBuoyancy();
        }

        public override void OnStartClient()
        {
            if (simulateOnClients && !isServer) InitialiseBuoyancy();
        }

        private void FixedUpdate()
        {
            if (!ready || WaterWaves.Instance == null) return;

            if (!isServer && !simulateOnClients) return;

            if (grabbable != null && grabbable.IsHeld) return;

            float t = (float)NetworkTime.time;

            foreach (Vector3 floatPoint in floatPoints)
            {
                Vector3 point = transform.TransformPoint(floatPoint);
                float depth = WaterWaves.Instance.GetHeight(point, t) - point.y;

                if (depth <= 0f) continue;

                Vector3 pointVelocity = rb.GetPointVelocity(point);

                float force = depth * strength - pointVelocity.y * dampingForce;

                if (force > 0f) rb.AddForceAtPosition(Vector3.up * force, point);

                float submersion = Mathf.Clamp01(depth / restDepth);

                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(pointVelocity, Vector3.up);
                Vector3 dragForce = -horizontalVelocity * horizontalDrag * submersion;

                rb.AddForceAtPosition(dragForce, point);
            }
        }
    }
}