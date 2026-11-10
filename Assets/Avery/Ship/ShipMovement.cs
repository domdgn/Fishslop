using Mirror;
using UnityEngine;

public class ShipMovement : NetworkBehaviour
{
    public float topSpeed = 10f;
    public float acceleration = 2f;
    public float deceleration = 1f;
    public float steerSpeed = 20f;

    private Rigidbody rb;
    private SailHandler sailHandler;
    private float steerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.useGravity = true;
        rb.angularDamping = 2f;
    }

    private void Start()
    {
        sailHandler = GetComponentInChildren<SailHandler>(true);
    }

    public override void OnStartServer()
    {
        rb.isKinematic = false;
    }

    public override void OnStartClient()
    {
        if (!isServer) rb.isKinematic = true;
    }

    [Server]
    public void SetSteerInput(float direction)
    {
        steerInput = Mathf.Clamp(direction, -1f, 1f);
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        if (sailHandler == null) return;

        Vector3 forward = -transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (sailHandler.isOpen)
        {
            rb.AddForce(forward * acceleration, ForceMode.Acceleration);
        }
        else
        {
            Vector3 slowed = Vector3.MoveTowards(flatVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(slowed.x, rb.linearVelocity.y, slowed.z);
        }

        float forwardSpeed = Vector3.Dot(flatVelocity, forward);
        if (forwardSpeed > topSpeed) rb.linearVelocity -= forward * (forwardSpeed - topSpeed);

        float speedFactor = Mathf.Clamp01(flatVelocity.magnitude / topSpeed);
        rb.AddTorque(Vector3.up * steerInput * steerSpeed * Mathf.Deg2Rad * speedFactor, ForceMode.Acceleration);
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;

        Debug.Log($"Ship crashed into {collision.gameObject.name} / Impact speed: {impactSpeed}");
    }
}