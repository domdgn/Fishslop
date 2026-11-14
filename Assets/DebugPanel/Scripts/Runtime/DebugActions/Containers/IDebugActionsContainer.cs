#nullable enable

using System;
using System.Collections.Generic;
using DebugPanel.DebugActions.Actions;

namespace DebugPanel.DebugActions.Containers
{
    public interface IDebugActionsContainer
    {
        IReadOnlyList<IDebugAction> DebugActions { get; }
        
        event Action<IDebugActionsContainer, IDebugAction>? OnAdded; 
        event Action<IDebugAction>? OnRemoved; 
        
        /// <summary>
        /// Adds a debug action to the current section. 
        /// </summary>
        /// <param name="action">The debug action to be added to the section.</param>
        void Add(IDebugAction action);
        
        /// <summary>
        /// Removes a debug action from the current section.
        /// </summary>
        /// <param name="action">The debug action to be removed from the section.</param>
        void Remove(IDebugAction action);

        /// <summary>
        /// Removes all debug actions from the current section.
        /// </summary>
        void Clear();
    }
}