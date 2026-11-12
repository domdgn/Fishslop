using UnityEngine;

namespace FishSlop
{
    public class ShipDeckFollower : MonoBehaviour
    {
        [SerializeField] private Transform ship;
        public Rigidbody shipRB => ship.GetComponent<Rigidbody>();
        public ShipMovement shipMovement => ship.GetComponent<ShipMovement>();

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (ship == null) return;

            rb.MovePosition(ship.position);
            rb.MoveRotation(ship.rotation);
        }
    }
}