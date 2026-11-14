#nullable enable

using System;
using System.Collections.Generic;
using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Popups;
using TMPro;
using DebugPanel.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace DebugPanel.Views
{
    public sealed class DebugPanelSectionSelectorPopup : DebugPanelPopup
    {
        [Header("Prefabs")] 
        public ListSelectorPopupElement? ElementPrefab;

        [Header("References")] 
        public Button? CloseButton;
        public ScrollRect ScrollRect;
        public RectTransform? ContentParent;
        public TMP_InputField? FilterInputField;
        public Button? ClearFilterButton;

        readonly List<ListSelectorPopupElement> _elementInstances = new();

        Action<DebugActionsSection>? _whenSelectedAction;
        Action? _whenCloseAction;
        
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

        public void Setup(
            IEnumerable<DebugActionsSection> sections, 
            Action<DebugActionsSection> whenSelectedAction,
            Action whenCloseAction
            )
        {
            _whenSelectedAction = whenSelectedAction;
            _whenCloseAction = whenCloseAction;
            
            foreach (ListSelectorPopupElement element in _elementInstances)
            {
                Destroy(element.gameObject);
            }
            
            _elementInstances.Clear();
            
            foreach (DebugActionsSection section in sections)
            {
                ListSelectorPopupElement elementInstance = Instantiate(ElementPrefab!, ContentParent);
                elementInstance.Label.text = section.Name;
                elementInstance.Button.onClick.AddListener(() => WhenElementSelected(section));
                _elementInstances.Add(elementInstance);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(ContentParent);

            ScrollRect.verticalNormalizedPosition = 0;
        }

        void WhenClosed()
        {
            _whenCloseAction?.Invoke();
            gameObject.SetActive(false);
        }
        
        void WhenElementSelected(DebugActionsSection section)
        {
            WhenClosed();
            _whenSelectedAction?.Invoke(section);
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