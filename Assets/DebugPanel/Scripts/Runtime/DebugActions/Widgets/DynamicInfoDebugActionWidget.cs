#nullable enable

using System;
using TMPro;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class DynamicInfoDebugActionWidget : DebugActionWidget
    {
        public TextMeshProUGUI? Label;
    
        Func<string>? _getInfoAction;
    
        public void Init(Func<string> getInfoAction)
        {
            _getInfoAction = getInfoAction;
        
            UpdateText();
        }

        void Update()
        {
            UpdateText();
        }

        void UpdateText()
        {
            if (_getInfoAction == null)
            {
                return;
            }
        
            string info = _getInfoAction.Invoke();
            Label!.text = info;
        }
    }
}