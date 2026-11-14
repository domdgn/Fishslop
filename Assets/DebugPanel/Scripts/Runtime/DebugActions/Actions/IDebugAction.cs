#nullable enable

using System;

namespace DebugPanel.DebugActions.Actions
{
    /// <summary>
    /// Represents a debug action that can be associated with a
    /// user interface widget in a debug panel.
    /// </summary>
    public interface IDebugAction
    {
        string ActionName { get; }
        string? Documentation { get; }

        IDebugAction SetDocumentation(string documentation);
        IDebugAction SetEnabledPredicate(Func<bool> predicate);
        IDebugAction SubscribeWhenRemoved(Action action);
    }
}