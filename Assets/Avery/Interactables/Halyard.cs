using UnityEngine;

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

    void Start()
    {

    }

    void Update()
    {

    }
}
