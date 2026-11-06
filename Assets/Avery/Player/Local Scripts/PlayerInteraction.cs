using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactMask;
    public Transform cameraPivot;
    private IInteractable lookedAt;
    public void DoRaycast()
    {
        if (cameraPivot == null) { return; }
        if (InteractableUIHandler.Instance == null) { Debug.LogWarning("hey no interact UI instance"); return; }

        Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);

        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactionDistance, interactMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                lookedAt = interactable;
                InteractableUIHandler.Instance.ShowInteractableText(interactable.GetInteractionText());
            }
            else
            {
                lookedAt = null;
                InteractableUIHandler.Instance.HideInteractableText();
            }
        }
        else
        {
            lookedAt = null;
            InteractableUIHandler.Instance.HideInteractableText();
        }
    }

    public void DoInteract()
    {
        Debug.Log($"PlayerInteraction: DoInteract called. {lookedAt}");

        if (lookedAt != null)
        {
            lookedAt.OnInteractStart(transform);
        }
        else
        {
            Debug.Log("No interactable object in range.");
        }
    }
}
