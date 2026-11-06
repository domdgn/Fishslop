using UnityEngine;

public class ShipWheel : MonoBehaviour, IInteractable
{
    public string interactionText = "{INTERACT} Helm ship";
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
