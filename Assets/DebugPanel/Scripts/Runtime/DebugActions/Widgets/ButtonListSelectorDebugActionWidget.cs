#nullable enable

using System;
using System.Collections.Generic;
using DebugPanel.Core;
using DebugPanel.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class ButtonListSelectorDebugActionWidget : DebugActionWidget
    {
        [Header("Prefabs")] 
        public ListSelectorPopup? ListSelectorPopup;
        
        [Header("References")]
        public TextMeshProUGUI? Label;
        public Button? Button;

        string _name = string.Empty;
        IReadOnlyList<string> _elements = Array.Empty<string>();
        Action<int>? _setAction;
        Func<int>? _getAction;
        bool _refreshEveryFrame;

        void Awake()
        {
            Button!.onClick.AddListener(WhenButtonClick);
        }

        public void Init(
            string widgetName, 
            IReadOnlyList<string> elements, 
            Action<int> setAction, 
            Func<int> getAction,
            bool refreshEveryFrame
        )
        {
            _name = widgetName;
            _elements = elements;
            _setAction = setAction;
            _getAction = getAction;
            _refreshEveryFrame = refreshEveryFrame;

            RefreshName();
        }

        void Update()
        {
            if (_refreshEveryFrame)
            {
                RefreshName();
            }
        }

        void WhenButtonClick()
        {
            UDebugPanel.Popups.AddPopup(popupParent =>
            {
                ListSelectorPopup instance = Instantiate(ListSelectorPopup!, popupParent);
                instance.Setup(_name, WhenElementSelected);
                instance.SetElements(_elements);
                return instance.gameObject;
            });
        }

        void WhenElementSelected(int index)
        {
            _setAction?.Invoke(index);
            
            RefreshName();
        }

        void RefreshName()
        {
            if (_elements.Count == 0)
            {
                return;
            }
            
            int index = _getAction!.Invoke();

            string element = _elements[index];
            
            Label!.text = $"{_name}: {element}";
        }
    }
}