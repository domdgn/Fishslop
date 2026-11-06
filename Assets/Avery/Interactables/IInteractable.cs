using UnityEngine;

public interface IInteractable
{ 
    void OnInteractEnd(Transform interactor);
    void OnInteractStart(Transform interactor);
    void OnLookAt(Transform interactor);

    string GetInteractionText();
}
