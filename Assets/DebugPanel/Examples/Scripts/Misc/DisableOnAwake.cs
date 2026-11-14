using UnityEngine;

namespace DebugPanel.Examples
{
    public sealed class DisableOnAwake : MonoBehaviour
    {
        void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}