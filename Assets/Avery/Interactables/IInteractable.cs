using Mirror;
using UnityEngine;

namespace FishSlop
{
    public interface IInteractable
    {
        void OnInteractEnd(Transform interactor);
        void OnInteractStart(Transform interactor);
        void OnLookAt(Transform interactor);

        NetworkIdentity GetNetworkIdentity();
        string GetInteractionText();
    }
}