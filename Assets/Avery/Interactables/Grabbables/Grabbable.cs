using Mirror;
using UnityEngine;

namespace FishSlop
{
    public class Grabbable : NetworkBehaviour, IInteractable
    {
        [SerializeField] private float followStrength = 15f;
        [SerializeField] private float maxSpeed = 12f;
        [SerializeField] private float holdDistance = 1.5f;

        [SyncVar(hook = nameof(OnHolderChanged))]
        private NetworkIdentity holder;

        public bool IsHeld => holder != null;

        private Rigidbody rb;
        private Collider[] cols;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            cols = GetComponentsInChildren<Collider>();
        }

        [Server]
        public void Grab(NetworkIdentity who)
        {
            var old = holder;
            holder = who;
            OnHolderChanged(old, who);
            rb.useGravity = false;
        }

        [Server]
        public void Release()
        {
            var old = holder;
            holder = null;
            OnHolderChanged(old, null);
            rb.useGravity = true;
        }

        [Server]
        public void Follow(Vector3 target)
        {
            PlayerLookHandler look = holder.GetComponent<PlayerLookHandler>();
            Vector3 pivot = look.headPivot.position;

            float pitch = look != null ? look.syncedPitch : 0f;

            Quaternion rotation = Quaternion.Euler(pitch, holder.transform.eulerAngles.y, 0f );

            Vector3 desiredPosition = pivot + rotation * Vector3.forward * holdDistance;

            Vector3 vel = (desiredPosition - rb.position) * followStrength;
            rb.linearVelocity = Vector3.ClampMagnitude(vel, maxSpeed);

            rb.angularVelocity = Vector3.zero;
            rb.rotation = rotation;
        }

        private void OnHolderChanged(NetworkIdentity oldHolder, NetworkIdentity newHolder)
        {
            SetIgnore(oldHolder, false);
            SetIgnore(newHolder, true);
        }

        private void SetIgnore(NetworkIdentity id, bool ignore)
        {
            if (id == null) return;
            foreach (var pc in id.GetComponentsInChildren<Collider>())
            {
                foreach (var c in cols) Physics.IgnoreCollision(c, pc, ignore);
            }
        }

        public void OnInteractStart(Transform interactor)
        {
            var pg = interactor.GetComponent<PlayerGrab>();
            if (pg == null) return;

            if (holder == pg.netIdentity) pg.Drop();
            else if (!IsHeld) pg.RequestGrab(netIdentity);
        }

        public void OnInteractEnd(Transform interactor) { }
        public void OnLookAt(Transform interactor) { }

        public string GetInteractionText()
        {
            if (!IsHeld) return "{INTERACT} Pick up";
            if (holder == NetworkClient.localPlayer) return "{INTERACT} Drop";
            return "";
        }

        public NetworkIdentity GetNetworkIdentity() => netIdentity;
    }
}