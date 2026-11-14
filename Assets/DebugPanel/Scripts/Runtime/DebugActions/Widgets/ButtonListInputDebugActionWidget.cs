#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DebugPanel.Core;
using DebugPanel.Popups;
using TMPro;
using DebugPanel.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class ButtonListInputDebugActionWidget : DebugActionWidget
    {
        [Header("Prefabs")] 
        public ListSelectorPopup? ListSelectorPopup;
        
        [Header("References")]
        public TextMeshProUGUI? Label;
        public Button? Button;
        
        Func<IReadOnlyList<string>>? _getElementsFunc;
        Func<Task<IReadOnlyList<string>>>? _getElementsFuncAsync;
        Action<int>? _elementSelectedAction;
        bool _closePanelOnInput;
        
        public void Init(
            string widgetName, 
            Func<IReadOnlyList<string>> getElementsFunc, 
            Action<int> elementSelectedAction,
            bool closePanelOnInput
            )
        {
            Label!.text = widgetName;
            Button!.onClick.AddListener(WhenButtonClick);
            
            _getElementsFunc = getElementsFunc;
            _elementSelectedAction = elementSelectedAction;
            _closePanelOnInput = closePanelOnInput;
        }
        
        public void Init(
            string widgetName, 
            Func<Task<IReadOnlyList<string>>> getElementsFunc, 
            Action<int> elementSelectedAction,
            bool closePanelOnInput
        )
        {
            Label!.text = widgetName;
            Button!.onClick.AddListener(WhenButtonClick);
            
            _getElementsFuncAsync = getElementsFunc;
            _elementSelectedAction = elementSelectedAction;
            _closePanelOnInput = closePanelOnInput;
        }

        Task<IReadOnlyList<string>> GetElementsAsync()
        {
            if (_getElementsFunc != null)
            {
                return Task.FromResult(_getElementsFunc.Invoke());
            }
            
            return _getElementsFuncAsync!.Invoke();
        }

        void WhenButtonClick()
        {
            UDebugPanel.Popups.AddPopup(popupParent =>
            {
                ListSelectorPopup instance = Instantiate(ListSelectorPopup!, popupParent);
                SetupPopupAsync(instance).FireTaskAndForget();
                return instance.gameObject;
            });
        }
        
        async Task SetupPopupAsync(ListSelectorPopup instance)
        {
            void WhenSelected(int index)
            {
                if (_closePanelOnInput)
                {
                    UDebugPanel.Hide();
                }
                
                _elementSelectedAction!.Invoke(index);
            }
            
            instance.Setup(Label!.text, WhenSelected);

            IReadOnlyList<string> elements = await GetElementsAsync();

            if (instance == null)
            {
                return;
            }
            
            instance.SetElements(elements);
        }
    }
}