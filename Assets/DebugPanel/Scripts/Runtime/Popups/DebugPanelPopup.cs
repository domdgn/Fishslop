#nullable enable

using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Popups
{
    public abstract class DebugPanelPopup : MonoBehaviour
    {
        /// <summary>
        /// When controller navigation is enabled on the debug panel,
        /// this function is called when showing the popup, and it should
        /// return the first element of the popup that should be focused.
        /// </summary>
        public virtual Selectable? GetFirstFocusedSelectable() => null;
        public virtual void WhenFocusedOnStack(){}
        public virtual void WhenUnfocusedFromStack(){}
    }
}