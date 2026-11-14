#nullable enable

using System;
using System.Collections.Generic;
using DebugPanel.DebugActions.Containers;
using DebugPanel.DebugActions.Widgets;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class ButtonActionsContainerDebugAction : DebugAction, IDebugActionsContainer
    {
        public IReadOnlyList<IDebugAction> DebugActions => _actions;
        
        public bool ClearOnInstantiate { get; set; }
        
        public event Action<IDebugActionsContainer, IDebugAction>? OnAdded;
        public event Action<IDebugAction>? OnRemoved;
        public event Action<IDebugActionsContainer>? OnInstantiate;
        
        readonly List<IDebugAction> _actions = new();
        
        public ButtonActionsContainerDebugAction(string actionName)
        {
            ActionName = actionName;
        }

        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            ButtonActionsContainerDebugActionWidget widget = (ButtonActionsContainerDebugActionWidget)viewInstance;
            widget.Init(
                ActionName,
                this,
                ClearOnInstantiate,
                OnInstantiate
            );
        }

        public void Add(IDebugAction action)
        {
            _actions.Add(action);
            OnAdded?.Invoke(this, action);
        }

        public void Remove(IDebugAction action)
        {
            bool found = _actions.Remove(action);

            if (!found)
            {
                return;
            }
        
            OnRemoved?.Invoke(action);
        }

        public void Clear()
        {
            foreach (IDebugAction debugAction in _actions)
            {
                OnRemoved?.Invoke(debugAction);
            }
            
            _actions.Clear();
        }
    }
}