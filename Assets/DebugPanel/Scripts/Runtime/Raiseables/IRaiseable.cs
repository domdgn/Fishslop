using System;

namespace DebugPanel.Raiseables
{
    public interface IRaiseable<T> : IListenRaiseable<T>
    {
        void Raise(T data);
    }
}