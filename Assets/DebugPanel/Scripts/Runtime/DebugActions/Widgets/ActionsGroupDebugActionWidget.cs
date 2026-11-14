#nullable enable

using System;
using System.Collections.Generic;
using DebugPanel.Core;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Containers;
using TMPro;
using UnityEngine;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class ActionsGroupDebugActionWidget : DebugActionWidget
    {
        [Header("References")]
        public TextMeshProUGUI? Label;
        public RectTransform? ActionsParent;
        
        readonly Dictionary<IDebugAction, DebugActionWidget> _widgets = new();

        public void Init(
            string groupName,
            IReadOnlyList<IDebugAction> currentActions,
            ref Action<IDebugActionsContainer, IDebugAction>? onAdded,
            ref Action<IDebugAction>? onRemoved
            )
        {
            Label!.text = groupName;

            onAdded += WhenAdded;
            onRemoved += WhenRemoved;

            CreateWidgets(currentActions);
        }

        void CreateWidgets(IReadOnlyList<IDebugAction> currentActions)
        {
            foreach (IDebugAction debugAction in currentActions)
            {
                if (!(debugAction is DebugAction castedDebugAction))
                {
                    return;
                }

                DebugActionWidget? widget = UDebugPanel.CoreCreateDebugActionWidget(
                    castedDebugAction,
                    ActionsParent!
                );

                if (widget != null)
                {
                    _widgets.Add(castedDebugAction, widget);
                }
            }
        }

        void WhenAdded(IDebugActionsContainer container, IDebugAction action)
        {
            if (!(action is DebugAction castedDebugAction))
            {
                return;
            }
            
            DebugActionWidget? widget = UDebugPanel.CoreCreateDebugActionWidget(
                castedDebugAction,
                ActionsParent!
            );

            if (widget != null)
            {
                _widgets.Add(castedDebugAction, widget);
            }
        }
        
        void WhenRemoved(IDebugAction action)
        {
            bool found = _widgets.TryGetValue(action, out DebugActionWidget? widget);

            if (!found)
            {
                return;
            }

            _widgets.Remove(action);
            Destroy(widget!.gameObject);
        }
    }
}