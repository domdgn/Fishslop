using Mirror;
using UnityEngine;

namespace FishSlop
{
    [RequireComponent(typeof(Rigidbody))]
    public class ShipFloat : NetworkBehaviour
    {
        [SerializeField] private Collider hullCollider;
        [SerializeField] private float restDepth = 0.5f;
        [SerializeField] private float damping = 0.4f;

        private Rigidbody rb;
        private Vector3[] floatPoints = new Vector3[4];
        private float strength;
        private float dampingForce;
        private bool ready;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (hullCollider == null) hullCollider = GetComponent<Collider>();
        }

        public override void OnStartServer()
        {
            Physics.SyncTransforms();

            Bounds b = hullCollider.bounds;
            floatPoints[0] = transform.InverseTransformPoint(new Vector3(b.min.x, b.min.y, b.min.z));
            floatPoints[1] = transform.InverseTransformPoint(new Vector3(b.max.x, b.min.y, b.min.z));
            floatPoints[2] = transform.InverseTransformPoint(new Vector3(b.min.x, b.min.y, b.max.z));
            floatPoints[3] = transform.InverseTransformPoint(new Vector3(b.max.x, b.min.y, b.max.z));

            strength = rb.mass * Physics.gravity.magnitude / (floatPoints.Length * restDepth);
            dampingForce = damping * 2f * Mathf.Sqrt(strength * rb.mass / floatPoints.Length);

            Debug.Log($"ShipFloat ready. bounds center {b.center}, size {b.size}, strength {strength:F0}");
            ready = true;
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            if (!ready || WaterWaves.Instance == null) return;

            float t = (float)NetworkTime.time;

            foreach (Vector3 floatPoint in floatPoints)
            {
                Vector3 point = transform.TransformPoint(floatPoint);
                float depth = WaterWaves.Instance.GetHeight(point, t) - point.y;
                if (depth <= 0f) continue;

                float force = depth * strength - rb.GetPointVelocity(point).y * dampingForce;
                if (force > 0f) rb.AddForceAtPosition(Vector3.up * force, point);
            }
        }
    }
}