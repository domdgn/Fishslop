using Mirror;
using UnityEngine;

namespace FishSlop
{
    public class ShipPush : NetworkBehaviour
    {
        [SerializeField] private Rigidbody shipRb;
        [SerializeField] private float pushForce;

        public void Push()
        {
            CmdPush();
        }

        [Command(requiresAuthority = false)]
        private void CmdPush(NetworkConnectionToClient sender = null)
        {
            Vector3 force = shipRb.transform.forward * pushForce;

            if (shipRb.linearVelocity.z < pushForce)
            {
                shipRb.AddForce(force, ForceMode.VelocityChange);
            }
        }
    }
}