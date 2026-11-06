using UnityEngine;
using Mirror;

public class WaveBobDisplacement : NetworkBehaviour
{
    private float baseHeight;

    private void Start()
    {
        baseHeight = transform.localPosition.y;
    }

    private void Update()
    {
        if (!isServer) return;

        Vector3 worldPosition = transform.position;
        float time = Time.time;

        Vector3 displacement = WaterWaves.Instance.GetDisplacement(worldPosition, time);

        Vector3 localPosition = transform.localPosition;
        localPosition.y = baseHeight + displacement.y;

        transform.localPosition = localPosition;
    }
}