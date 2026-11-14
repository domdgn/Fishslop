using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using UnityEngine;

namespace DebugPanel.Examples
{
    /// <summary>
    /// Example demonstrating how to setup documentation for the
    /// debug actions you add to the panel. This documentation will be
    /// available through the blue Doc button found on the top right part
    /// of the debug panel.
    /// </summary>
    public sealed class DebugPanelDocumentationExample : MonoBehaviour
    {
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.SetControllerSupport(true);
            UDebugPanel.Show();

            IDebugActionsSection section = UDebugPanel.AddSection("Documented Section");
            
            section.AddButton("Some Button", () => Debug.Log("Hi"))
                .SetDocumentation("A button that prints 'Hi' to the console when pressed");
            
            section.AddInfo("Hello Debug Panel :)")
                .SetDocumentation("A text that says 'Hello Debug Panel'");

            section.AddInfo("Press the Doc button");
        }
    }
}