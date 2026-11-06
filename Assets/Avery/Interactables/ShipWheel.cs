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
        PlayerSteerShip playerSteerShip = interactor.GetComponent<PlayerSteerShip>();
        if (playerSteerShip != null)
        {
            ShipMovement shipMovement = GetComponentInParent<ShipMovement>();
            if (shipMovement != null)
            {
                playerSteerShip.StartSteeringShip(shipMovement);
            }
            else
            {
                Debug.LogError("ShipMovement component not found in parent of " + gameObject.name);
            }
        }
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
