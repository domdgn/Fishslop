using System;
using DebugPanel.Logic;
using DebugPanel.Views;
using UnityEngine;

namespace DebugPanel.PopupBuilders
{
    public sealed class PopupContainer : IPopupContainer
    {
        readonly PopupsLogic _popupsLogic;

        public PopupContainer(PopupsLogic popupsLogic)
        {
            _popupsLogic = popupsLogic;
        }
        
        public void AddPopup(Func<Transform, GameObject> spawnPopupAction)
        {
            _popupsLogic.AddPopup(spawnPopupAction); 
        }

        public void AddPopup(Func<Transform, DebugPanelView, GameObject> spawnPopupAction)
        {
            _popupsLogic.AddPopup(spawnPopupAction); 
        }

        public void RemovePopup(GameObject popup)
        {
            _popupsLogic.RemovePopup(popup);
        }
        
        public void RemoveCurrentPopup()
        {
            _popupsLogic.RemoveCurrentPopup();
        }
    }
}