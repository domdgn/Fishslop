using System.Collections.Generic;
using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;
using UnityEngine;

namespace DebugPanel.Examples
{
    /// <summary>
    /// Example demonstrating the usage of the different widgets
    /// that can be used on the Debug Panel.
    /// </summary>
    public sealed class DebugPanelWidgetsExample : MonoBehaviour
    {
        int _dynamicInfo;
        bool _toggle;
        int _intSelector;
        float _floatSelector;
        long _longSelector;
        int _elementListSelected;
        string _stringSelector = "Empty";
        TestEnum _elementEnumSelected;
        string _stringInput = "Empty";

        readonly List<string> _elementsList = new()
        {
            "Element1",
            "Element2",
            "Element3",
            "Element4",
            "Element5",
            "Element6",
            "Element7",
            "Element8",
            "Element9",
        };

        enum TestEnum
        {
            Element1,
            Element2,
            Element3,
            Element4,
        }
        
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.SetControllerSupport(true);
            UDebugPanel.Show();

            IDebugActionsSection section = UDebugPanel.AddSection("Section");
            
            // Info
            section.AddInfo("Info");
            section.AddInfoDynamic(() => $"Info dynamic {_dynamicInfo}");
            
            // Buttons
            section.AddButton("Button", () => Debug.Log("Button pressed"));
            section.AddButtonLargeInfo("Button Large Info", () => "This is some large info");
            section.AddButtonListInput("Button List Input", () => _elementsList, i => Debug.Log($"Selected {_elementsList[i]}"));
            section.AddButtonListSelector("Button List Selector", _elementsList, i => _elementListSelected = i, () => _elementListSelected);
            section.AddButtonEnumSelector("Enum Selector", i => _elementEnumSelected = i, () => _elementEnumSelected);
            section.AddButtonStringInput("Button String Input", () => _stringInput, v => _stringInput = v);
            section.AddButtonStringSelector("Button String Selector", v => _stringSelector = v, () => _stringSelector);
            
            // Toggle
            section.AddToggle("Toggle", v => _toggle = v, () => _toggle);
            
            // Selectors
            section.AddIntSelector("Int Selector", v => _intSelector = v, () => _intSelector);
            section.AddFloatSelector("Float Selector", v => _floatSelector = v, () => _floatSelector);
            section.AddLongSelector("Long Selector", v => _longSelector = v, () => _longSelector);
            
            // Container
            section.AddButtonActionsContainer("Actions", s =>
            {
                s.AddButton("Another Button 1", () => Debug.Log("Button pressed"));
                s.AddButton("Another Button 2", () => Debug.Log("Button pressed"));
                s.AddButton("Another Button 3", () => Debug.Log("Button pressed"));
            });
        }
        
        void Update()
        {
            ++_dynamicInfo;
        }
    }
}