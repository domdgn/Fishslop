using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using UnityEngine;

namespace DebugPanel.Examples
{
    /// <summary>
    /// Example demonstrating how debug panel sections behave.
    /// </summary>
    public sealed class DebugPanelSectionsExample : MonoBehaviour
    {
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.SetControllerSupport(true);
            UDebugPanel.Show();

            for (int i = 1; i < 100; i++)
            {
                IDebugActionsSection section = UDebugPanel.AddSection($"Section {i}");
                AddActionsToSection(section);
            }
        }

        static void AddActionsToSection(IDebugActionsSection section1)
        {
            section1.AddButton("Button", () => { });
            section1.AddToggle("Toggle", b => { }, () => true);
            section1.AddInfo("Some info about the game");
            section1.AddInfo("Time left: 32h 15m 3s");
            section1.AddInfo($"Enabled: <color=#1AE680>true</color>");
            section1.AddInfo("Long info can also be added without a problem because it wraps properly");
            section1.AddButton("Select Element", () => { });
            section1.AddIntSelector("Health", i => { }, () => 32);
            section1.AddFloatSelector("Power Strenght", i => { }, () => 12.4f);
            section1.AddToggle("Invincible", b => { }, () => false);
            section1.AddInfo("Mode: Battle");
            section1.AddInfo("Enemies: Wandering");
            section1.AddButton("Reset progress", () => { });
            section1.AddInfo("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");

            for (int i = 0; i < 10; i++)
            {
                section1.AddButton($"Button {i}", () => { });
            }
        }
    }
}