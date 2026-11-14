#nullable enable

using System;

namespace DebugPanel.Raiseables
{
    public sealed class Raiseable<T> : IRaiseable<T>
    {
        public event Action<T>? WhenRaised;
        
        public void Raise(T data)
        {
            WhenRaised?.Invoke(data);
        }
    }
}