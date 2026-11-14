using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace DebugPanel.Input
{
    public sealed class SpawnAppropiateInputSystem : MonoBehaviour
    {
        void Awake()
        {
#if ENABLE_INPUT_SYSTEM 
            gameObject.AddComponent<InputSystemUIInputModule>();
#else
            gameObject.AddComponent<StandaloneInputModule>();
#endif
        }
    }
}