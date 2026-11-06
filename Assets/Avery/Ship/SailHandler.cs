using UnityEngine;

public class SailHandler : MonoBehaviour
{
    public Vector3 openPos;
    public Vector3 openScale;
    public Vector3 closedPos;
    public Vector3 closedScale;
    public bool isOpen = false;
    public float lerpSpeed = 5f;

    private void Update()
    {
        if (isOpen)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, openPos, Time.deltaTime * lerpSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, openScale, Time.deltaTime * lerpSpeed);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, closedPos, Time.deltaTime * lerpSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, closedScale, Time.deltaTime * lerpSpeed);
        }
    }

    public void ToggleSail()
    {
        isOpen = !isOpen;
    }
}
