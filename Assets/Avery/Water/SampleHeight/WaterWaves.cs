using UnityEngine;

public class WaterWaves : MonoBehaviour
{
    public static WaterWaves Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public GerstnerWave wave1;
    public GerstnerWave wave2;
    public GerstnerWave wave3;

    public Vector3 GetDisplacement(Vector3 worldPosition, float time)
    {
        return wave1.GetDisplacement(worldPosition, time) + wave2.GetDisplacement(worldPosition, time) + wave3.GetDisplacement(worldPosition, time);
    }

    public float GetHeight(Vector3 worldPosition, float time)
    {
        return transform.position.y + GetDisplacement(worldPosition, time).y;
    }
}