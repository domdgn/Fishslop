#nullable enable

using System;
using System.Collections.Generic;
using DebugPanel.DebugActions.Actions;

namespace DebugPanel.DebugActions.Containers
{
    public sealed class DebugActionsSection : IDebugActionsSection, IDisposable
    {
        readonly List<IDebugAction> _actions = new();
        
        readonly Action<DebugActionsSection>? _refreshAction;
    
        public string Name { get; }
        public int Priority { get; }

        public IReadOnlyList<IDebugAction> DebugActions => _actions;
        
        public event Action<IDebugActionsContainer, IDebugAction>? OnAdded;
        public event Action<IDebugAction>? OnRemoved;
    
        public DebugActionsSection(
            string name, 
            int priority,
            Action<DebugActionsSection> refreshAction
        )
        {
            Name = name;
            Priority = priority;
            _refreshAction = refreshAction;
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

        public void Refresh()
        {
            _refreshAction?.Invoke(this);
        }

        public void Dispose()
        {
            OnRemoved = null;
        }
    }
}