using UnityEngine;
using Mirror;
using PolyverseSkiesAsset;

public class TimeOfDayHandler : NetworkBehaviour
{
    public PolyverseSkies sky;

    [SerializeField] private Color dayFogColour = new Color32(206, 193, 73, 255);
    [SerializeField] private Color nightFogColour = Color.black;

    [SyncVar]
    private double dayStartTime;

    public float dayDuration = 300;

    [Range(0f, 1f)]
    public float timeOfDay;

    public override void OnStartServer()
    {
        dayStartTime = NetworkTime.time;
    }

    private void Update()
    {
        if (!isClient && !isServer) return;
        if (dayDuration <= 0f) return;

        double elapsed = NetworkTime.time - dayStartTime;

        timeOfDay = Mathf.PingPong((float)(elapsed / dayDuration), 1f);

        sky.timeOfDay = timeOfDay;

        RenderSettings.fogColor = Color.Lerp(dayFogColour, nightFogColour, timeOfDay);
    }
}
