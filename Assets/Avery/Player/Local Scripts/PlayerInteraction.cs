
using UnityEngine;
using Mirror;

namespace FishSlop
{
    public class PlayerInteraction : NetworkBehaviour
    {
        public float interactionDistance = 3f;
        [SerializeField] private LayerMask interactMask;

        public Transform cameraPivot;

        private IInteractable lookedAt;

        private void Update()
        {
            if (!isOwned) return;

            DoRaycast();
        }

        public void DoRaycast()
        {
            if (cameraPivot == null) return;
            if (InteractableUIHandler.Instance == null) return;

            Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);

            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);

            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactMask))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    lookedAt = interactable;
                    InteractableUIHandler.Instance.ShowInteractableText(interactable.GetInteractionText());

                    return;
                }
            }

            lookedAt = null;
            InteractableUIHandler.Instance.HideInteractableText();
        }
        private string GetRelativePath(Transform child, Transform parent)
        {
            if (child == parent) return "";

            string path = child.name;
            while (child.parent != parent)
            {
                child = child.parent;
                path = child.name + "/" + path;
            }
            return path;
        }

        public void DoInteract()
        {
            if (!isOwned || lookedAt == null) return;

            NetworkIdentity identity = lookedAt.GetNetworkIdentity();
            if (identity == null) return;

            Transform interactableTransform = ((MonoBehaviour)lookedAt).transform;

            string path = GetRelativePath(interactableTransform, identity.transform);

            CmdInteract(identity, path);
        }

        [Command]
        private void CmdInteract(NetworkIdentity target, string path)
        {
            if (target == null) return;

            Transform child = string.IsNullOrEmpty(path) ? target.transform : target.transform.Find(path);
            if (child == null) return;

            IInteractable interactable = child.GetComponent<IInteractable>();
            if (interactable == null) return;

            if (Vector3.Distance(transform.position, child.position) > interactionDistance + 1f) return;

            interactable.OnInteractStart(transform);
        }
    }
}