#nullable enable

using System;
using DebugPanel.Core;
using TMPro;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class ToggleDebugActionWidget : DebugActionWidget
    {
        public TextMeshProUGUI? Label;
        public Toggle? Toggle;

        Action<bool>? _setAction;
        Func<bool>? _getAction;
        bool _closePanelOnInput;
    
        public void Init(string widgetName, Action<bool> setAction, Func<bool> getAction, bool closePanelOnInput)
        {
            _setAction = setAction;
            _getAction = getAction;
            _closePanelOnInput = closePanelOnInput;
        
            Label!.text = widgetName;
            Toggle!.isOn = _getAction!.Invoke();
            Toggle.onValueChanged.AddListener(WhenToggle);
        }
        
        void WhenToggle(bool val)
        {
            if (_closePanelOnInput)
            {
                UDebugPanel.Hide();
            }
            
            bool newState = Toggle!.isOn;
            _setAction!.Invoke(newState);
            Toggle.SetIsOnWithoutNotify(_getAction!.Invoke());
        }

        public override void Refresh()
        {
            bool newState = _getAction!.Invoke();
            Toggle!.SetIsOnWithoutNotify(newState);
        }
    }
}