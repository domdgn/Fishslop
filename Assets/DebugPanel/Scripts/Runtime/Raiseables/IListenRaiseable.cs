#nullable enable

using System;

namespace DebugPanel.Raiseables
{
    public interface IListenRaiseable<out T>
    {
        event Action<T>? WhenRaised; 
    }
}