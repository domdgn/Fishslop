namespace DebugPanel.DebugActions.Containers
{
    /// <summary>
    /// Represents a section within a debug actions UI, where various debug
    /// actions can be added or removed.
    /// </summary>
    public interface IDebugActionsSection : IDebugActionsContainer
    {
        /// <summary>
        /// Prompts a refresh of all the underlying debug actions.
        /// </summary>
        void Refresh();
    }
}