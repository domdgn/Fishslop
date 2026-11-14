using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using UnityEngine;

namespace DebugPanel.Examples
{
    /// <summary>
    /// Example demonstrating how controller navigation works on the
    /// debug panel.
    /// </summary>
    public sealed class DebugPanelControllerNavigationExample : MonoBehaviour
    {
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.SetControllerSupport(true);
            UDebugPanel.Show();
            
            for (int i = 0; i < 30; i++)
            {
                AddActionsToSection(UDebugPanel.AddSection($"Section {i}"));
            }
        }

        static void AddActionsToSection(IDebugActionsContainer section)
        {
            section.AddButtonActionsContainer("Actions", AddActionsToSection);
            
            for (int i = 0; i < 70; i++)
            {
                section.AddButton($"Button {i}", () => Debug.Log("Button pressed"));
            }
        }
    }
}