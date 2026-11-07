using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : NetworkBehaviour
{
    public float topSpeed = 10f;
    public float acceleration = 2f;
    public float steerSpeed = 20f;
    private float currentSpeed = 0f;

    private SailHandler sailHandler;

    private float steerInput;

    [Server]
    public void SetSteerInput(float direction)
    {
        steerInput = Mathf.Clamp(direction, -1f, 1f);
    }

    private void Start()
    {
        sailHandler = GetComponentInChildren<SailHandler>(true);
    }

    void FixedUpdate()
    {
        if (!isServer) return;

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

        if (NetworkServer.active)
        {
            Steer(steerInput);
        }
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

    public void Steer(float direction)
    {
        if (!NetworkServer.active || currentSpeed <= 0) return;

        transform.Rotate(Vector3.up, direction * steerSpeed * (currentSpeed / topSpeed) * Time.fixedDeltaTime);
    }
}
