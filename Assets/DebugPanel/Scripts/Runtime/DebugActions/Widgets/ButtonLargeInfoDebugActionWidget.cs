#nullable enable
using System;
using System.Threading.Tasks;
using DebugPanel.Popups;
using TMPro;
using DebugPanel.Behaviours;
using DebugPanel.Core;
using DebugPanel.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class ButtonLargeInfoDebugActionWidget : DebugActionWidget
    {
        [Header("Prefabs")] 
        public LargeTextPopup? LargeTextPopup;
        
        [Header("References")]
        public TextMeshProUGUI? Label;
        public Button? Button;

        RectTransform? _popupsParent;
        Func<string>? _infoAction;
        Func<Task<string>>? _infoAsyncAction;
        bool _refreshEveryFrame;

        void Awake()
        {
            Button!.onClick.AddListener(WhenButtonClick);
        }

        public void Init(
            string widgetName, 
            Func<string> infoAction,
            bool refreshEveryFrame
        )
        {
            Init(widgetName);
            
            _infoAction = infoAction;
            _refreshEveryFrame = refreshEveryFrame;
        }
        
        public void Init(
            string widgetName, 
            Func<Task<string>> infoAction
        )
        {
            Init(widgetName);
            
            _infoAsyncAction = infoAction;
        }

        void Init(string widgetName)
        {
            Label!.text = widgetName;
        }

        void WhenButtonClick()
        {
            SetupPopup();
        }
        
        void SetupPopup()
        {
            UDebugPanel.Popups.AddPopup(parent =>
            {
                LargeTextPopup popupInstance = Instantiate(LargeTextPopup!, parent);

                if (_infoAction != null)
                {
                    popupInstance.Setup(Label!.text, _infoAction, _refreshEveryFrame);
                }
                else
                {
                    popupInstance.Setup(Label!.text, _infoAsyncAction!, _refreshEveryFrame);
                }
                
                return popupInstance.gameObject;
            });
        }
    }
}