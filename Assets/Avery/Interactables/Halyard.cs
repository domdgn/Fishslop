using Mirror;
using UnityEngine;

namespace FishSlop
{
    public class Halyard : MonoBehaviour, IInteractable
    {
        public string interactionText = "{INTERACT} Lower/Raise the sail";
        public SailHandler sailHandler;
        public void OnInteractEnd(Transform interactor)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteractStart(Transform interactor)
        {
            sailHandler.ToggleSail();
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
            NetworkIdentity identity = GetComponentInParent<NetworkIdentity>();
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
}