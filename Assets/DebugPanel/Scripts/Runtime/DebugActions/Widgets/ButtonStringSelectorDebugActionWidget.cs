#nullable enable

using System;
using DebugPanel.Core;
using DebugPanel.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class ButtonStringSelectorDebugActionWidget : DebugActionWidget
    {
        [Header("Prefabs")] 
        public TextInputPopup? TextInputPopup;
        
        [Header("References")]
        public TextMeshProUGUI? Label;
        public Button? Button;

        string _widgetName = string.Empty;
        Action<string>? _setAction;
        Func<string>? _getAction;

        void Awake()
        {
            Button!.onClick.AddListener(WhenButtonClick);
        }

        public void Init(
            string widgetName, 
            Action<string> setAction,
            Func<string> getAction
        )
        {
            _widgetName = widgetName;
            _setAction = setAction;
            _getAction = getAction;
            
            Label!.text = $"{_widgetName}: {_getAction.Invoke()!}";
            
        }

        void WhenButtonClick()
        {
            UDebugPanel.Popups.AddPopup(popupParent =>
            {
                TextInputPopup instance = Instantiate(TextInputPopup!, popupParent);
                instance.Setup(Label!.text, _getAction?.Invoke()!, WhenStringSelected);
                return instance.gameObject;
            });
        }
        
        void WhenStringSelected(string value)
        {
            _setAction?.Invoke(value);
            string finalValue = _getAction?.Invoke()!;
            Label!.text = $"{_widgetName}: {finalValue}";
        }
    }
}