#nullable enable

using System;
using DebugPanel.Core;
using DebugPanel.Datas;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Containers;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Views;
using UnityEngine;

namespace DebugPanel.Logic
{
    public sealed class DebugActionsWidgetsLogic
    {
        readonly DebugPanelData _debugPanelData;
        readonly DebugActionsData _debugActionsData;
        readonly DebugActionsPrefabsData _debugActionsPrefabsData;
        readonly ControllerNavigationLogic _controllerNavigationLogic;

        public DebugActionsWidgetsLogic(
            DebugPanelData debugPanelData, 
            DebugActionsData debugActionsData, 
            DebugActionsPrefabsData debugActionsPrefabsData, 
            ControllerNavigationLogic controllerNavigationLogic
            )
        {
            _debugPanelData = debugPanelData;
            _debugActionsData = debugActionsData;
            _debugActionsPrefabsData = debugActionsPrefabsData;
            _controllerNavigationLogic = controllerNavigationLogic;
        }

        public void LinkBaseDebugActionsWithTheirWidgets(DebugPanelView debugPanelView)
        {
            UDebugPanel.RegisterWidgetPrefab<ButtonDebugAction>(debugPanelView.ButtonDebugActionWidgetPrefab);
            UDebugPanel.RegisterWidgetPrefab<InfoDebugAction>(debugPanelView.InfoDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<DynamicInfoDebugAction>(debugPanelView.DynamicInfoDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<ToggleDebugAction>(debugPanelView.ToggleDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<IntSelectorDebugAction>(debugPanelView.IntSelectorDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<LongSelectorDebugAction>(debugPanelView.LongSelectorDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<FloatSelectorDebugAction>(debugPanelView.FloatSelectorDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<ButtonListInputDebugAction>(debugPanelView.ButtonListInputDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<ButtonListSelectorDebugAction>(debugPanelView.ButtonListSelectorDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<ButtonLargeInfoDebugAction>(debugPanelView.ButtonLargeInfoDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<ButtonLargeInfoAsyncDebugAction>(debugPanelView.ButtonLargeInfoDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<ButtonConsoleDebugAction>(debugPanelView.ButtonConsoleDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<ButtonActionsContainerDebugAction>(debugPanelView.ButtonActionsContainerDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<ButtonStringInputDebugAction>(debugPanelView.ButtonStringInputDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<ButtonStringSelectorDebugAction>(debugPanelView.ButtonStringSelectorDebugActionWidget);
            UDebugPanel.RegisterWidgetPrefab<ActionsGroupDebugAction>(debugPanelView.ActionsGroupDebugActionWidget);
        }

        public void ClearDebugActionsWidgetsLists()
        {
            _debugActionsData.Widgets.Clear();
        }

        public DebugActionWidget? CreateDebugActionWidget(
            DebugAction debugAction,
            RectTransform parent
            )
        {
            Type actionType = debugAction.GetType();

            bool prefabFound = _debugActionsPrefabsData.DebugActionTypeByDebugActionWidgetPrefabs.TryGetValue(
                actionType,
                out DebugActionWidget? debugActionWidgetPrefab
            );

            if (!prefabFound)
            {
                Debug.LogError($"Widget prefab not found for debug action {actionType.Name}");
                return null;
            }

            if (debugActionWidgetPrefab == null)
            {
                Debug.LogError($"Widget prefab was null for debug action {actionType.Name}");
                return null;
            }

            DebugActionWidget instance = UnityEngine.Object.Instantiate(debugActionWidgetPrefab, parent)!;
            
            debugAction.InitWidget(
                instance
            );
            
            _controllerNavigationLogic.SetupControllerNavigation(instance.gameObject);
            
            return instance;
        }

        public void CreateDebugActionWidgetOnMainPanel(IDebugAction debugAction)
        {
            bool hasInstance = _debugPanelData.DebugPanelView != null;

            if (!hasInstance)
            {
                return;
            }

            DebugPanelView debugPanelView = _debugPanelData.DebugPanelView!;

            if (!(debugAction is DebugAction castedDebugAction))
            {
                return;
            }
            
            bool canBeShown = castedDebugAction.EnabledPredicate == null || castedDebugAction.EnabledPredicate.Invoke();

            if (!canBeShown)
            {
                return;
            }
            
            bool alreadyExists = _debugActionsData.Widgets.ContainsKey(debugAction);

            if (alreadyExists)
            {
                return;
            }

            DebugActionWidget? instance = CreateDebugActionWidget(
                castedDebugAction,
                debugPanelView!.WidgetsContentParent
            );

            if (instance == null)
            {
                return;
            }

            _debugActionsData.Widgets.Add(debugAction, instance);
        }

        public void CreateDebugActionWidgetOnMainPanelIfFromSelectedSection(
            IDebugActionsContainer debugActionSection,
            IDebugAction debugAction
            )
        {
            bool isSelectedSection = _debugPanelData.SelectedSection == debugActionSection;

            if (!isSelectedSection)
            {
                return;
            }
            
            CreateDebugActionWidgetOnMainPanel(debugAction);
        }

        public void DestroyMainPanelDebugActionWidget(IDebugAction debugAction)
        {
            bool hasWidget = _debugActionsData.Widgets.TryGetValue(debugAction, out DebugActionWidget widget);

            if (!hasWidget)
            {
                return;
            }

            _debugActionsData.Widgets.Remove(debugAction);

            if (widget != null)
            {
                UnityEngine.Object.Destroy(widget.gameObject);   
            }
        }

        public void DestroyAllMainPanelDebugActionWidgets()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            foreach (DebugActionWidget debugActionWidget in _debugActionsData.Widgets.Values)
            {
                UnityEngine.Object.Destroy(debugActionWidget.gameObject);
            }
            
            _debugActionsData.Widgets.Clear();
        }

        public void RefreshDebugActionWidgetInstanceLife(IDebugAction debugAction)
        {
            bool hasInstance = _debugPanelData.DebugPanelView != null;

            if (!hasInstance)
            {
                return;
            }

            if (!(debugAction is DebugAction castedDebugAction))
            {
                return;
            }
        
            bool canBeShown = castedDebugAction.EnabledPredicate == null || castedDebugAction.EnabledPredicate.Invoke();
            bool alreadyExists = _debugActionsData.Widgets.ContainsKey(debugAction);

            bool shouldDestroy = alreadyExists && !canBeShown;
            bool shouldCreate = !alreadyExists && canBeShown;

            if (shouldDestroy)
            {
                DestroyMainPanelDebugActionWidget(debugAction);
            }
            else if (shouldCreate)
            {
                CreateDebugActionWidgetOnMainPanel(debugAction);
            }
        }
    }
}