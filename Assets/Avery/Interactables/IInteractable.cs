using Mirror;
using UnityEngine;

public interface IInteractable
{ 
    void OnInteractEnd(Transform interactor);
    void OnInteractStart(Transform interactor);
    void OnLookAt(Transform interactor);

    NetworkIdentity GetNetworkIdentity();
    string GetInteractionText();
}
