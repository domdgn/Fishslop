#nullable enable

using System.Collections.Generic;
using DebugPanel.Core;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Containers;
using DebugPanel.DebugActions.Widgets;
using TMPro;
using DebugPanel.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Popups
{
    public sealed class ActionsContainerPopup : DebugPanelPopup
    {
        [Header("References")] 
        public TextMeshProUGUI? TitleLabel;
        public Button? CloseButton;
        public RectTransform? ContentParent;
        public TMP_InputField? FilterInputField;
        public Button? ClearFilterButton;

        readonly List<(DebugActionWidget widget, DebugAction action)> _widgets = new();
        
        IDebugActionsContainer? _debugActionsContainer;

        void Awake()
        {
            CloseButton!.onClick.AddListener(WhenClosed);
            FilterInputField!.onValueChanged.AddListener(WhenFilterChanged);
            ClearFilterButton!.onClick.AddListener(WhenClearFilterButtonPressed);
        }

        public override Selectable? GetFirstFocusedSelectable()
        {
            foreach ((DebugActionWidget widget, DebugAction action) element in _widgets)
            {
                Selectable? selectable = element.widget.GetComponentInChildren<Selectable>();

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
        
        public void Setup(string title, IDebugActionsContainer debugActionsContainer)
        {
            TitleLabel!.text = title;
            _debugActionsContainer = debugActionsContainer;

            CreateWidgets();
        }
        
        void WhenClosed()
        {
            UDebugPanel.Popups.RemovePopup(gameObject);
        }

        void CreateWidgets()
        {
            foreach (IDebugAction debugAction in _debugActionsContainer!.DebugActions)
            {
                if (!(debugAction is DebugAction castedDebugAction))
                {
                    return;
                }

                DebugActionWidget? widget = UDebugPanel.CoreCreateDebugActionWidget(
                    castedDebugAction,
                    ContentParent!
                );

                if (widget != null)
                {
                    _widgets.Add((widget, castedDebugAction));
                }
            }
        }
        void WhenFilterChanged(string newText)
        {
            string filterText = newText.ToLowerNoSpaces();
            
            foreach ((DebugActionWidget widget, DebugAction action) element in _widgets)
            {
                string toCheck = element.action.ActionName.ToLowerNoSpaces();

                bool canShow = toCheck.Contains(filterText);

                element.widget.gameObject.SetActive(canShow);
            }
        }

        void WhenClearFilterButtonPressed()
        {
            FilterInputField!.text = string.Empty;
        }
    }
}