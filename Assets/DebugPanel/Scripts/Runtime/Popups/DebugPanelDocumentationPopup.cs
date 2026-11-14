#nullable enable

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Views
{
    public sealed class DebugPanelDocumentationPopup : MonoBehaviour
    {
        [Header("References")] 
        public TextMeshProUGUI? TitleLabel;
        public Button? CloseButton;
        public TextMeshProUGUI? TextLabel;

        Action? _whenCloseAction;
        
        void Awake()
        {
            CloseButton!.onClick.AddListener(WhenClosed);
        }
        
        public void Setup(string title, string text, Action whenCloseAction)
        {
            TitleLabel!.text = title;
            TextLabel!.text = text;
            _whenCloseAction = whenCloseAction;
        }

        void WhenClosed()
        {
            _whenCloseAction?.Invoke();
            gameObject.SetActive(false);
        }
    }
}