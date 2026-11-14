#nullable enable

using System;
using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class ButtonActionsContainerDebugActionWidget : DebugActionWidget
    {
        [Header("Prefabs")] 
        public ActionsContainerPopup? ActionsContainerPopup;
        
        [Header("References")]
        public TextMeshProUGUI? Label;
        public Button? Button;

        IDebugActionsContainer? _debugActionsContainer;
        bool _clearOnInstantiate;
        Action<IDebugActionsContainer>? _instantiateAction;

        public void Init(
            string widgetName,
            IDebugActionsContainer debugActionsContainer,
            bool clearOnInstantiate,
            Action<IDebugActionsContainer>? instantiateAction
            )
        {
            Label!.text = widgetName;
            Button!.onClick.AddListener(WhenButtonClick);

            _debugActionsContainer = debugActionsContainer;
            _clearOnInstantiate = clearOnInstantiate;
            _instantiateAction = instantiateAction;
        }

        void WhenButtonClick()
        {
            if (_clearOnInstantiate)
            {
                _debugActionsContainer!.Clear();
            }
            
            _instantiateAction!.Invoke(_debugActionsContainer!);
            
            UDebugPanel.Popups.AddPopup(popupParent =>
            {
                ActionsContainerPopup instance = Instantiate(ActionsContainerPopup!, popupParent);
                instance.Setup(Label!.text, _debugActionsContainer!);
                return instance.gameObject;
            });
        }
    }
}