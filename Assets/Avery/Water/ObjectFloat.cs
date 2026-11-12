using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace FishSlop
{
    [RequireComponent(typeof(Rigidbody))]
    public class ObjectFloat : NetworkBehaviour
    {
        public List<Transform> floatTransforms = new List<Transform>();

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        [ServerCallback]
        private void FixedUpdate()
        {
            if (floatTransforms.Count < 3) return;

            Vector3 centre = Vector3.zero;

            foreach (Transform t in floatTransforms) centre += t.position;

            centre /= floatTransforms.Count;

            Vector3 normal = Vector3.zero;

            for (int i = 0; i < floatTransforms.Count; i++)
            {
                Vector3 a = floatTransforms[i].position - centre;
                Vector3 b = floatTransforms[(i + 1) % floatTransforms.Count].position - centre;

                normal += Vector3.Cross(a, b);
            }

            if (normal.sqrMagnitude < 0.0001f) return;

            normal.Normalize();

            if (normal.y < 0) normal = -normal;

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, normal) * rb.rotation;

            rb.MovePosition(centre);
            rb.MoveRotation(targetRotation);
        }
    }
}