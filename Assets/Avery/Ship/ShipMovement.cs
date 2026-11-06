using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float topSpeed = 10f;
    public float acceleration = 2f;
    private float currentSpeed = 0f;

    private SailHandler sailHandler;

    private void Start()
    {
        sailHandler = GetComponentInChildren<SailHandler>(true);
    }

    void FixedUpdate()
    {
        if (sailHandler == null) {
            Debug.LogError("SailHandler component not found in children of " + gameObject.name);
            return; 
        }

        if (sailHandler.isOpen)
        {
            Accelerate();
        }
        else
        {
            Decelerate();
        }

        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);
    }

    private void Accelerate()
    {
        if (currentSpeed < topSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            if (currentSpeed > topSpeed)
            {
                currentSpeed = topSpeed;
            }
        }
    }

    private void Decelerate()
    {
        if (currentSpeed > 0)
        {
            currentSpeed -= acceleration * Time.deltaTime;
            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }
    }
}
