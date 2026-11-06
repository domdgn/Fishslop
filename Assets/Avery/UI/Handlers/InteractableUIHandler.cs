using TMPro;
using UnityEngine;

public class InteractableUIHandler : MonoBehaviour
{
    public static InteractableUIHandler Instance { get; private set; }
    public TextMeshProUGUI InteractableText;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ShowInteractableText(string text)
    {
        InteractableText.text = text;
        InteractableText.gameObject.SetActive(true);
    }


    public void HideInteractableText()
    {
        InteractableText.gameObject.SetActive(false);
    }
}
