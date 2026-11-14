using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using UnityEngine;

namespace DebugPanel.Examples
{
    public sealed class DebugPanelActionsGroupExample : MonoBehaviour
    {
        bool _godMode;
        int _health = 100;
        
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.SetControllerSupport(true);
            UDebugPanel.Show();

            IDebugActionsSection section = UDebugPanel.AddSection("Section");
            
            section.AddButton("Complete Mission",() => { Debug.Log("Complete Mission"); });
            section.AddButton("Go To Main Menu",() => { Debug.Log("Go To Main Menu"); });
            
            IDebugActionsContainer group = section.AddActionsGroup("Player");
            group.AddToggle("God Mode", v => _godMode = v, () => _godMode);
            group.AddIntSelector("Health", v => _health = v, () => _health);
            group.AddButton("Kill", () => { Debug.Log("Kill Player"); });
            group.AddInfo("This is a very long string inside a group that wraps properly");
            
            section.AddInfoDynamic(() => "Ping: 32ms");
            section.AddInfoDynamic(() => "Fps: 60");
        }
    }
}