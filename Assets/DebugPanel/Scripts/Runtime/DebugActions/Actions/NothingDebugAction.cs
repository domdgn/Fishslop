#nullable enable

using System;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class NothingDebugAction : IDebugAction
    {
        public static readonly NothingDebugAction Instance = new();
        
        public string ActionName => string.Empty;
        public string? Documentation => null;
        
        NothingDebugAction() {}

        public IDebugAction SetDocumentation(string documentation) => this;
        public IDebugAction SetEnabledPredicate(Func<bool> predicate) => this;
        public IDebugAction SubscribeWhenRemoved(Action action) => this;
    }
}