#nullable enable

using System.Collections.Generic;
using DebugPanel.Datas;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Containers;
using DebugPanel.DebugActions.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Logic
{
    public sealed class ControllerNavigationLogic
    {
        static readonly List<Selectable> CachedSelectablesList = new();
        
        readonly ControllersData _controllersData;
        readonly DebugActionsData _debugActionsData;
        readonly DebugPanelData _debugPanelData;

        public ControllerNavigationLogic(
            ControllersData controllersData, 
            DebugActionsData debugActionsData, 
            DebugPanelData debugPanelData
            )
        {
            _controllersData = controllersData;
            _debugActionsData = debugActionsData;
            _debugPanelData = debugPanelData;
        }

        public void SetControllerSupport(bool set)
        {
            _controllersData.ControllerSupport = set;
        }

        public bool HasControllerSupportEnabled()
        {
            return _controllersData.ControllerSupport;
        }

        public void SetupControllerNavigation(GameObject gameObject)
        {
            gameObject.GetComponentsInChildren(CachedSelectablesList);

            foreach (Selectable selectable in CachedSelectablesList)
            {
                Navigation navigation = selectable.navigation;
                navigation.mode = _controllersData.ControllerSupport ? Navigation.Mode.Automatic : Navigation.Mode.None;
                selectable.navigation = navigation;
            }
        }

        public bool Select(GameObject gameObject)
        {
            if (!_controllersData.ControllerSupport)
            {
                return false;
            }
            
            gameObject.GetComponentsInChildren(CachedSelectablesList);
            
            foreach (Selectable selectable in CachedSelectablesList)
            {
                selectable.Select();
                return true;
            }

            return false;
        }
        
        public void SelectFirstSectionWidget(DebugActionsSection? section)
        {
            if (section == null)
            {
                return;
            }
            
            if (!_controllersData.ControllerSupport)
            {
                return;
            }
            
            foreach (IDebugAction action in section.DebugActions)
            {
                bool viewFound = _debugActionsData.Widgets.TryGetValue(
                    action,
                    out DebugActionWidget? widget
                );

                if (!viewFound)
                {
                    continue;
                }

                bool couldSelect = Select(widget!.gameObject);

                if (couldSelect)
                {
                    return;
                }
            }
            
            Select(_debugPanelData.DebugPanelView!.FilterInputField.gameObject);
        }
    }
}