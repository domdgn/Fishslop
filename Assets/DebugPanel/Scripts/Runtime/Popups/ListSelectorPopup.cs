#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DebugPanel.Core;
using TMPro;
using DebugPanel.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Popups
{
    public sealed class ListSelectorPopup : DebugPanelPopup
    {
        [Header("Prefabs")] 
        public ListSelectorPopupElement? ElementPrefab;

        [Header("References")] 
        public TextMeshProUGUI? TitleLabel;
        public Button? CloseButton;
        public RectTransform? ContentParent;
        public TMP_InputField? FilterInputField;
        public Button? ClearFilterButton;
        public GameObject? LoadingBlock;

        readonly List<ListSelectorPopupElement> _elementInstances = new();
        Action<int>? _elementSelectedAction;

        void Awake()
        {
            CloseButton!.onClick.AddListener(WhenClosed);
            FilterInputField!.onValueChanged.AddListener(WhenFilterChanged);
            ClearFilterButton!.onClick.AddListener(WhenClearFilterButtonPressed);
        }

        public override Selectable? GetFirstFocusedSelectable()
        {
            foreach (ListSelectorPopupElement element in _elementInstances)
            {
                Selectable? selectable = element.GetComponentInChildren<Selectable>();

                if (selectable != null)
                {
                    return selectable;
                }
            }

            return null;
        }
        
        public override void WhenFocusedOnStack()
        {
            if (Application.isEditor && !UDebugPanel.HasControllerSupportEnabled())
            {
                FilterInputField!.text = string.Empty;
                FilterInputField.Select();
                FilterInputField.ActivateInputField();
            }
        }
        
        public void Setup(string title, Action<int> elementSelectedAction)
        {
            _elementSelectedAction = elementSelectedAction;
            
            TitleLabel!.text = title;
            
            LoadingBlock!.SetActive(true);
        }

        public void SetElements(IReadOnlyList<string> getElementsFunc)
        {
            IEnumerable<(string value, int index)> elements = getElementsFunc.Select((value, index) => (value, index)).OrderBy(o => o.index);

            foreach ((string value, int index) element in elements)
            {
                ListSelectorPopupElement elementInstance = Instantiate(ElementPrefab!, ContentParent);
                elementInstance.Label.text = element.value;
                elementInstance.Button.onClick.AddListener(() => WhenElementSelected(element.index));
                
                _elementInstances.Add(elementInstance);
            }
            
            LoadingBlock!.SetActive(false);
        }

        void WhenClosed()
        {
            UDebugPanel.Popups.RemovePopup(gameObject);
        }
        
        void WhenElementSelected(int index)
        {
            UDebugPanel.Popups.RemovePopup(gameObject);
            _elementSelectedAction?.Invoke(index);
        }

        void WhenFilterChanged(string newText)
        {
            string filterText = newText.ToLowerNoSpaces();
            
            foreach (ListSelectorPopupElement elementInstance in _elementInstances)
            {
                string toCheck = elementInstance.Label.text.ToLowerNoSpaces();

                bool canShow = toCheck.Contains(filterText);

                elementInstance.gameObject.SetActive(canShow);
            }
        }

        void WhenClearFilterButtonPressed()
        {
            FilterInputField!.text = string.Empty;
        }
    }
}