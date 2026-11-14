#nullable enable
using System;
using System.Collections.Generic;
using DebugPanel.Core;
using DebugPanel.Popups;
using DebugPanel.Raiseables;
using TMPro;
using DebugPanel.Behaviours;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class ButtonConsoleDebugActionWidget : DebugActionWidget
    {
        [Header("Prefabs")] 
        public ConsolePopup? ConsolePopup;
        
        [Header("References")]
        public TextMeshProUGUI? Label;
        public Button? Button;

        Func<IReadOnlyList<string>>? _logsAction;
        IListenRaiseable<string>? _addLogRaiseable;
        
        public void Init(
            string widgetName, 
            Func<IReadOnlyList<string>> logsAction,
            IListenRaiseable<string>? addLogRaiseable
        )
        {
            Label!.text = widgetName;
            Button!.onClick.AddListener(WhenButtonClick);
            
            _logsAction = logsAction;
            _addLogRaiseable = addLogRaiseable;
        }

        void WhenButtonClick()
        {
            SetupPopup();
        }
        
        void SetupPopup()
        {
            UDebugPanel.Popups.AddPopup(popupParent =>
            {
                ConsolePopup instance = Instantiate(ConsolePopup!, popupParent);
                instance.Setup(Label!.text, _logsAction!.Invoke(), _addLogRaiseable);
                return instance.gameObject;
            });
        }
    }
}