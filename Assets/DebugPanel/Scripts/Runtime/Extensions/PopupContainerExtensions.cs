#nullable enable
using System;
using System.Collections.Generic;
using DebugPanel.DebugActions.Containers;
using DebugPanel.PopupBuilders;
using DebugPanel.Popups;
using DebugPanel.Raiseables;
using Object = UnityEngine.Object;

namespace DebugPanel.Extensions
{
    public static class PopupContainerExtensions
    {
        public static void AddActionsContainerPopup(
            this IPopupContainer popupContainer, 
            string title,
            Action<IDebugActionsContainer> setupActions
            )
        {
            IDebugActionsContainer container = new SimpleDebugActionsContainer();
            setupActions.Invoke(container);

            popupContainer.AddActionsContainerPopup(
                title,
                container
            );
        }
        
        public static void AddActionsContainerPopup(
            this IPopupContainer popupContainer, 
            string title,
            IDebugActionsContainer container
        )
        {
            popupContainer.AddPopup((popupParent, view) =>
            {
                ActionsContainerPopup instance = Object.Instantiate(view.ActionsContainerPopup, popupParent);
                instance.Setup(title, container);
                
                return instance.gameObject; 
            });
        }
        
        public static void AddConsolePopup(
            this IPopupContainer popupContainer, 
            string title,
            IReadOnlyList<string> getLogs,
            IRaiseable<string>? addLog = null
        )
        {
            popupContainer.AddPopup((popupParent, view) =>
            {
                ConsolePopup instance = Object.Instantiate(view.ConsolePopup, popupParent);
                instance.Setup(title, getLogs, addLog);
                return instance.gameObject;
            });
        }
    }
}