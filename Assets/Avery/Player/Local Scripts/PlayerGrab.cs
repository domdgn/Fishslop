using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishSlop
{
    public class PlayerGrab : NetworkBehaviour
    {
        [SerializeField] private Vector3 holdOffset = new Vector3(0f, 1.2f, 1.5f);
        private float reach;
        private PlayerInteraction interaction;
        private Grabbable held;

        private void Awake()
        {
            interaction = GetComponent<PlayerInteraction>();
            reach = interaction.interactionDistance + 1.5f;
        }

        [Server]
        public void RequestGrab(NetworkIdentity target)
        {
            if (held != null || target == null) return;

            var g = target.GetComponent<Grabbable>();
            if (g == null || g.IsHeld) return;
            if (Vector3.Distance(transform.position, target.transform.position) > reach) return;

            held = g;
            held.Grab(netIdentity);
        }

        [Server]
        public void Drop()
        {
            if (held == null) return;
            held.Release();
            held = null;
        }

        [Command]
        private void CmdDrop() => Drop();

        [ServerCallback]
        private void FixedUpdate()
        {
            if (held != null)
            {
                held.Follow(transform.TransformPoint(holdOffset));
                if (Vector3.Distance(transform.position, held.transform.position) > reach)
                {
                    Drop();
                }
            }
        }

        public override void OnStopServer() => Drop();
    }
}