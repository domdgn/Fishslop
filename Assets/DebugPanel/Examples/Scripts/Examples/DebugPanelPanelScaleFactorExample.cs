using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using UnityEngine;

namespace DebugPanel.Examples
{
    public sealed class DebugPanelPanelScaleFactorExample : MonoBehaviour
    {
        float _scaleFactor = 1;
        
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.Show();

            IDebugActionsSection section = UDebugPanel.AddSection("Section");
            section.AddFloatSelector(
                "Scale", 
                0.1f,
                v =>
                {
                    UDebugPanel.SetPanelScaleFactor(v);
                    _scaleFactor = v;
                }, 
                () => _scaleFactor
            );
        }
    }
}