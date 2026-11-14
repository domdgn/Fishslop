#nullable enable

using System;
using DebugPanel.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Popups
{
    public sealed class TextInputPopup : DebugPanelPopup
    {
        [Header("References")] 
        public TextMeshProUGUI? TitleLabel;
        public Button? CloseButton;
        public TMP_InputField? InputField;
        public Button? AcceptButton;

        Action<string>? _acceptAction;

        void Awake()
        {
            CloseButton!.onClick.AddListener(WhenClosed);
            AcceptButton!.onClick.AddListener(WhenAccept);
        }

        public override Selectable? GetFirstFocusedSelectable()
        {
            return InputField!.GetComponentInChildren<Selectable>();
        }
        
        public void Setup(string title, string initialText, Action<string> acceptAction)
        {
            TitleLabel!.text = title;
            InputField!.text = initialText;
            _acceptAction = acceptAction;
            
            if (Application.isEditor && !UDebugPanel.HasControllerSupportEnabled())
            {
                InputField.Select();
                InputField.ActivateInputField();
                InputField.selectionAnchorPosition = 0;
                InputField.selectionFocusPosition = InputField.text.Length;
            }
        }
        
        void WhenClosed()
        {
            UDebugPanel.Popups.RemovePopup(gameObject);
        }

        void WhenAccept()
        {
            string input = InputField!.text;
            _acceptAction?.Invoke(input);
            WhenClosed();
        }
    }
}