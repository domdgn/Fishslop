#nullable enable

using System;
using DebugPanel.Core;
using DebugPanel.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Examples
{
    public sealed class ExampleCustomPopup : DebugPanelPopup
    {
        [Header("References")] 
        public TextMeshProUGUI? TitleLabel;
        public Button? CloseButton;
        public Button? DoSomethingButton;
        
        void Awake()
        {
            CloseButton!.onClick.AddListener(WhenClosed);
        }

        public override Selectable? GetFirstFocusedSelectable()
        {
            return DoSomethingButton;
        }

        public void Setup(string title, Action doSomethingAction)
        {
            TitleLabel!.text = title;
            DoSomethingButton!.onClick.AddListener(doSomethingAction.Invoke);
        }
        
        void WhenClosed()
        {
            UDebugPanel.Popups.RemovePopup(gameObject);
        }
    }
}