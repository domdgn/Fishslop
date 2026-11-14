#nullable enable

using System;
using DebugPanel.Core;
using TMPro;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class ButtonDebugActionWidget : DebugActionWidget
    {
        public TextMeshProUGUI? Label;
        public Button? Button;

        Action? _action;
        bool _closePanelOnInput;
        
        public void Init(string widgetName, Action action, bool closePanelOnInput)
        {
            _action = action;
            _closePanelOnInput = closePanelOnInput;
            
            Label!.text = widgetName;
            Button!.onClick.AddListener(WhenClick);
        }

        void WhenClick()
        {
            if (_closePanelOnInput)
            {
                UDebugPanel.Hide();
            }
            
            _action?.Invoke();
        }
    }
}