#nullable enable

using System;
using System.Threading.Tasks;
using DebugPanel.Constants;
using DebugPanel.Core;
using DebugPanel.Datas;
using DebugPanel.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DebugPanel.Logic
{
    public sealed class DebugPanelViewLogic
    {
        static DebugPanelView? _debugPanelViewPrefab;

        readonly DebugPanelData _debugPanelData;
        readonly DebugActionsWidgetsLogic _debugActionsWidgetsLogic;
        readonly ControllerNavigationLogic _controllerNavigationLogic;
        readonly ExtraPopupsLogic _extraPopupsLogic;
        readonly PreviousSectionLogic _previousSectionLogic;

        public DebugPanelViewLogic(
            DebugPanelData debugPanelData, 
            DebugActionsWidgetsLogic debugActionsWidgetsLogic, 
            ControllerNavigationLogic controllerNavigationLogic, 
            ExtraPopupsLogic extraPopupsLogic, 
            PreviousSectionLogic previousSectionLogic
            )
        {
            _debugPanelData = debugPanelData;
            _debugActionsWidgetsLogic = debugActionsWidgetsLogic;
            _controllerNavigationLogic = controllerNavigationLogic;
            _extraPopupsLogic = extraPopupsLogic;
            _previousSectionLogic = previousSectionLogic;
        }

        public async Task InstantiateDebugPanelView()
        {
            if (_debugPanelData.DebugPanelView != null)
            {
                return;
            }
            
            _debugPanelData.IsVisible = true;

            if (_debugPanelViewPrefab == null)
            {
                TaskCompletionSource<DebugPanelView?> taskCompletionSource = new();

                ResourceRequest resourceRequest = Resources.LoadAsync<DebugPanelView>(ResourcesConstants.ResourcesFilename);

                void Completed(AsyncOperation _)
                {
                    taskCompletionSource.SetResult(resourceRequest.asset as DebugPanelView);
                }

                resourceRequest.completed += Completed;

                _debugPanelViewPrefab = await taskCompletionSource.Task;
            }

            if (_debugPanelViewPrefab == null)
            {
                return;
            }

            if (_debugPanelData.DebugPanelView != null)
            {
                return;
            }

            DebugPanelView debugPanelView = Object.Instantiate(_debugPanelViewPrefab);
            Object.DontDestroyOnLoad(debugPanelView.gameObject);

            debugPanelView.VersionLabel.text = UDebugPanel.Version;
            
            _debugActionsWidgetsLogic.LinkBaseDebugActionsWithTheirWidgets(debugPanelView);

            debugPanelView.CloseButton.onClick.AddListener(UDebugPanel.Hide);
            debugPanelView.SectionSelectorButton.onClick.AddListener(_extraPopupsLogic.ShowSectionSelectorPopup);
            debugPanelView.SectionDocumentationButton.onClick.AddListener(_extraPopupsLogic.ShowDocumentationPopup);
            debugPanelView.PreviousSectionButton.onClick.AddListener(_previousSectionLogic.SelectPreviousSection);

            _debugPanelData.DebugPanelView = debugPanelView;

            debugPanelView.ShortcutButton.gameObject.SetActive(_debugPanelData.ShortcutAction != null);

            if (_debugPanelData.ShortcutAction != null)
            {
                debugPanelView.ShortcutLabel.text = _debugPanelData.ShortcutName;
                debugPanelView.ShortcutButton.onClick.AddListener(_debugPanelData.ShortcutAction.Invoke);
            }
            
            _controllerNavigationLogic.SetupControllerNavigation(debugPanelView.gameObject);

            if (Application.isEditor && !UDebugPanel.HasControllerSupportEnabled())
            {
                _debugPanelData.DebugPanelView.FilterInputField.Select();
                _debugPanelData.DebugPanelView.FilterInputField.ActivateInputField();
            }
            
            ApplyScaleFactorToView();
            ApplySortOrderToView();
        }

        public void DestroyDebugPanelView()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            DebugPanelView debugPanelView = _debugPanelData.DebugPanelView;

            _debugPanelData.DebugPanelView = null;
            _debugPanelData.IsVisible = false;

            if (!_debugPanelData.IsLoadingSections)
            {
                _debugPanelData.SectionsScrollPosition = debugPanelView.SectionsContentParent.anchoredPosition.y;
            }
            
            if (!_debugPanelData.IsLoadingWidgets)
            {
                _debugPanelData.WidgetsScrollPosition = debugPanelView.WidgetsContentParent.anchoredPosition.y;
            }
            
            Object.Destroy(debugPanelView.gameObject);
        }

        public bool IsVisible()
        {
            return _debugPanelData.IsVisible;
        }
        
        public bool IsInstantiated()
        {
            return _debugPanelData.DebugPanelView != null;
        }
        
        public void SetShortcutAction(string name, Action action)
        {
            _debugPanelData.ShortcutName = name;
            _debugPanelData.ShortcutAction = action;
        }

        public void SetScaleFactor(float set)
        {
            float inverted = 1f / Mathf.Max(0.01f, set);
            _debugPanelData.ScaleFactor = inverted;
            ApplyScaleFactorToView();
        }
        
        public void SetSortOrder(int set)
        {
            _debugPanelData.SortOrder = set;
            ApplySortOrderToView();
        }

        public void ApplyScaleFactorToView()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            CanvasScaler? canvasScaler = _debugPanelData.DebugPanelView.GetComponent<CanvasScaler>();

            if (canvasScaler == null)
            {
                return;
            }

            Vector2 newReferenceResolution = ScaleConstants.CanvasReferenceResolution * _debugPanelData.ScaleFactor;

            canvasScaler.referenceResolution = newReferenceResolution;
        }
        
        public void ApplySortOrderToView()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            Canvas? canvas = _debugPanelData.DebugPanelView.GetComponent<Canvas>();

            if (canvas == null)
            {
                return;
            }
            
            canvas.sortingOrder = _debugPanelData.SortOrder;
        }
    }
}