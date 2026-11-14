using System.Collections.Generic;
using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using UnityEngine;

namespace DebugPanel.Examples
{
    public sealed class DebugPanelManuallyShowingPopupsExample : MonoBehaviour
    {
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.Show();

            IDebugActionsSection section = UDebugPanel.AddSection("Section");
            
            section.AddButton("Show Actions Container Popup", () =>
            {
                UDebugPanel.Popups.AddActionsContainerPopup("Popup Title", container =>
                {
                    container.AddInfo("I can add actions here");
                    container.AddButton("I'm a button", () => Debug.Log("Hi"));
                });
            });
            
            section.AddButton("Show Console Popup", () =>
            {
                List<string> logs = new()
                {
                    "This is a log",
                    "Another log",
                    "And another log"
                };
                
                UDebugPanel.Popups.AddConsolePopup("Popup Title", logs);
            });
        }
    }
}