using DebugPanel.Core;
using UnityEngine;

namespace DebugPanel.Behaviours
{
    public sealed class DebugPanelToggleBehaviour : MonoBehaviour
    {
        void Update()
        {
            UDebugPanel.CoreTickToggleInput();
        }
    }
}