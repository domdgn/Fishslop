using System.Threading.Tasks;

namespace DebugPanel.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Runs a task asynchronously.
        /// Any exceptions thrown by the task are automatically logged to the Unity console.
        /// </summary>
        /// <param name="task">The task to run.</param>
        public static async void FireTaskAndForget(this Task task)
        {
            await task;
        }
    }
}