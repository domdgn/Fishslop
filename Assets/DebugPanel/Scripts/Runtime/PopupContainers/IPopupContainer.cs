using System;
using DebugPanel.Views;
using UnityEngine;

namespace DebugPanel.PopupBuilders
{
    public interface IPopupContainer
    {
        /// <summary>
        /// Adds a popup to the debug panel.
        /// This method provides the caller with the Transform that should be
        /// set up as the parent of the popup. Then the popup instance must
        /// be returned.
        /// </summary>
        /// <param name="spawnPopupAction">A function that takes a Transform and
        /// returns a GameObject representing the popup.</param>
        void AddPopup(Func<Transform, GameObject> spawnPopupAction);
        
        void AddPopup(Func<Transform, DebugPanelView, GameObject> spawnPopupAction);
        
        /// <summary>
        /// Removes a popup from the debug panel.
        /// The popup GameObject will automatically be destroyed by this method.
        /// </summary>
        void RemovePopup(GameObject popup);
        
        /// <summary>
        /// Removes the currently showing popup to the debug panel.
        /// The popup GameObject will automatically be destroyed by this method.
        /// </summary>
        void RemoveCurrentPopup();
    }
}