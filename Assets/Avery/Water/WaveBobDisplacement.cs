using UnityEngine;
using Mirror;

public class WaveBobDisplacement : NetworkBehaviour
{
    private float baseHeight;

    private void Start()
    {
        baseHeight = transform.position.y;
    }

    private void Update()
    {
        if (!isServer) return;

        Vector3 worldPosition = transform.position;
        float time = Time.time;

        Vector3 displacement = WaterWaves.Instance.GetDisplacement(worldPosition, time);
        float height = baseHeight + displacement.y;

        transform.position = new Vector3(worldPosition.x, height, worldPosition.z);
    }
}
