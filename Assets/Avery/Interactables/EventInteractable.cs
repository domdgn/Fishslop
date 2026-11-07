using Mirror;
using UnityEngine;
using UnityEngine.Events;
public class EventInteractable : MonoBehaviour, IInteractable
{
    public string interactionText = "{INTERACT} Interactable";

    public UnityEvent onInteractStart;
    public UnityEvent onInteractEnd;
    public void OnInteractEnd(Transform interactor)
    {
        onInteractEnd?.Invoke();
    }

    public void OnInteractStart(Transform interactor)
    {
        onInteractStart?.Invoke();
    }

    public void OnLookAt(Transform interactor)
    {
        throw new System.NotImplementedException();
    }

    public string GetInteractionText()
    {
        return interactionText;
    }

    public NetworkIdentity GetNetworkIdentity()
    {
        NetworkIdentity identity = GetComponent<NetworkIdentity>();
        if (identity == null)
        {
            identity = GetComponentInParent<NetworkIdentity>();
        }
        if (identity == null)
        {
            Debug.LogError("TemplateInteractable does not have a NetworkIdentity component.");
            return null;
        }

        return identity;
    }

    void Update()
    {
        
    }
}
