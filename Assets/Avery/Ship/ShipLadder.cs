using UnityEngine;

namespace FishSlop
{
    public class ShipLadder : MonoBehaviour
    {
        public ShipMovement ship;
        private void Awake() => ship = GetComponentInParent<ShipMovement>();

        private void OnTriggerEnter(Collider other)
        {
            var move = other.GetComponentInParent<PlayerMovementHandler>();
            if (move != null && move.isOwned) move.currentLadder = this;
        }

        private void OnTriggerExit(Collider other)
        {
            var move = other.GetComponentInParent<PlayerMovementHandler>();
            if (move != null && move.currentLadder == this) move.currentLadder = null;
        }
    }
}