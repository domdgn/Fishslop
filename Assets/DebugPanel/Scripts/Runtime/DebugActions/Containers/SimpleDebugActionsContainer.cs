#nullable enable
using System;
using System.Collections.Generic;
using DebugPanel.DebugActions.Actions;

namespace DebugPanel.DebugActions.Containers
{
    public sealed class SimpleDebugActionsContainer : IDebugActionsContainer
    {
        public IReadOnlyList<IDebugAction> DebugActions => _actions;
        
        public event Action<IDebugActionsContainer, IDebugAction>? OnAdded;
        public event Action<IDebugAction>? OnRemoved;
        
        readonly List<IDebugAction> _actions = new();
        
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