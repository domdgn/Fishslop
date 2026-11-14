#nullable enable

using System;
using DebugPanel.Core;
using DebugPanel.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class ButtonStringInputDebugActionWidget : DebugActionWidget
    {
        [Header("Prefabs")] 
        public TextInputPopup? TextInputPopup;
        
        [Header("References")]
        public TextMeshProUGUI? Label;
        public Button? Button;

        Func<string>? _initialTextAction;
        Action<string>? _inputAction;

        void Awake()
        {
            Button!.onClick.AddListener(WhenButtonClick);
        }

        public void Init(
            string widgetName, 
            Func<string> initialTextAction,
            Action<string> inputAction
            )
        {
            Label!.text = widgetName;

            _initialTextAction = initialTextAction;
            _inputAction = inputAction;
        }

        void WhenButtonClick()
        {
            UDebugPanel.Popups.AddPopup(popupParent =>
            {
                TextInputPopup instance = Instantiate(TextInputPopup!, popupParent);
                instance.Setup(Label!.text, _initialTextAction?.Invoke()!, _inputAction!);
                return instance.gameObject;
            });
        }
    }
}