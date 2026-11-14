using DebugPanel.Core;
using UnityEngine;

namespace DebugPanel.Examples
{
    /// <summary>
    /// Example demonstrating how to setup the top right shortcut button
    /// that can be used to quickly perform some functionality.
    /// </summary>
    public sealed class DebugPanelShortcutExample : MonoBehaviour
    {
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.SetControllerSupport(true);
            UDebugPanel.Show();

            UDebugPanel.SetShortcutAction("Shortcut", () =>
            {
                Debug.Log("Do something with shortcut");
            });
        }
    }
}