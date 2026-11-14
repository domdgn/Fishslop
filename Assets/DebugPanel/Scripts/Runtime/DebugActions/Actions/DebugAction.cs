#nullable enable
using System;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Views;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public abstract class DebugAction : IDebugAction
    {
        public Action? WhenRemoved;

        public virtual string ActionName { get; protected set; } = string.Empty;
        public string? Documentation { get; private set; }
        public Func<bool>? EnabledPredicate { get; private set; }

        public IDebugAction SetDocumentation(string documentation)
        {
            Documentation = documentation;
            return this;
        }

        public IDebugAction SetEnabledPredicate(Func<bool> predicate)
        {
            EnabledPredicate = predicate;
            return this;
        }

        public IDebugAction SubscribeWhenRemoved(Action action)
        {
            WhenRemoved += action;
            return this;
        }

        /// <summary>
        /// Instantiates a user interface widget for the debug action within the provided <see cref="DebugPanelView"/>.
        /// The instantiated control parent is automatically set after this, so it's not needed to manually set it on this method.
        /// </summary>
        /// <param name="viewInstance">The view where the debug action's widget will be instantiated.</param>
        /// <returns>The instantiated user interface widget for the debug action.</returns>
        public abstract void InitWidget(
            DebugActionWidget viewInstance
        );
    }
}