using UnityEngine;
using Mirror;

namespace FishSlop
{
    public class PlayerRespawn : NetworkBehaviour
    {
        [SerializeField] private float fallYLevel;

        private Rigidbody rb;
        private bool respawnPending;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!isOwned || respawnPending) return;

            if (rb.position.y < fallYLevel)
            {
                respawnPending = true;
                CmdRequestRespawn();
            }
        }

        [Command]
        public void CmdRequestRespawn()
        {
            GameObject respawnObject = GameObject.FindGameObjectWithTag("Respawn");

            if (respawnObject == null)
            {
                TargetRespawnFailed(connectionToClient);
                return;
            }

            Transform spawnPoint = respawnObject.transform;

            TargetRespawn(connectionToClient, spawnPoint.position, spawnPoint.rotation);
        }

        [TargetRpc]
        private void TargetRespawn(NetworkConnectionToClient target, Vector3 position, Quaternion rotation)
        {
            rb.position = position;
            rb.rotation = rotation;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            respawnPending = false;
        }

        [TargetRpc]
        private void TargetRespawnFailed(NetworkConnectionToClient target)
        {
            respawnPending = false;
        }
    }
}
