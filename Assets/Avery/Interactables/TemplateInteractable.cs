using Mirror;
using UnityEngine;

public class TemplateInteractable : MonoBehaviour, IInteractable
{
    public string interactionText = "{INTERACT} Interactable";
    public void OnInteractEnd(Transform interactor)
    {
        throw new System.NotImplementedException();
    }

    public void OnInteractStart(Transform interactor)
    {
        throw new System.NotImplementedException();
    }

    public void OnLookAt(Transform interactor)
    {
        throw new System.NotImplementedException();
    }

    public NetworkIdentity GetNetworkIdentity()
    {
        NetworkIdentity identity = GetComponentInParent<NetworkIdentity>();
        if (identity == null)
        {
            Debug.LogError("TemplateInteractable does not have a NetworkIdentity component.");
            return null;
        }
        
        return identity;
    }

    public string GetInteractionText()
    {
        return interactionText;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
