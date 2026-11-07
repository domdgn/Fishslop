
using UnityEngine;
using Mirror;

namespace FishSlop
{
    public class PlayerInteraction : NetworkBehaviour
    {
        [SerializeField] private float interactionDistance = 3f;
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

        public void DoInteract()
        {
            if (!isOwned || lookedAt == null) return;

            NetworkIdentity identity = lookedAt.GetNetworkIdentity();

            if (identity == null) return;

            CmdInteract(identity);
        }

        [Command]
        private void CmdInteract(NetworkIdentity target)
        {
            if (target == null) return;

            IInteractable interactable = target.GetComponent<IInteractable>();

            if (interactable == null) interactable = target.GetComponentInChildren<IInteractable>();

            if (interactable == null) return;

            if (Vector3.Distance(transform.position, target.transform.position) > interactionDistance + 1f) return;

            interactable.OnInteractStart(transform);
        }
    }
}