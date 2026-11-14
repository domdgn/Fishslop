#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using DebugPanel.Datas;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Containers;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Logic;
using DebugPanel.Extensions;
using DebugPanel.PopupBuilders;
using UnityEngine;

namespace DebugPanel.Core
{
    public sealed class UDebugPanel
    {
        public const string Version = "v1.3.3";
        
        static UDebugPanel? _instance;
        static UDebugPanel Instance => _instance ??= new UDebugPanel();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            _instance = new UDebugPanel();
            OnVisibilityStateChanged = null;
        }

        public static event Action<bool>? OnVisibilityStateChanged;

        public static IPopupContainer Popups => Instance._popupContainer;
        
        readonly DebugPanelData _debugPanelData = new();
        readonly DebugActionsData _debugActionsData = new();
        readonly DebugActionsPrefabsData _debugActionsPrefabsData = new();
        readonly PopupsData _popupsData = new();
        readonly ControllersData _controllersData = new();
        readonly InputData _inputData = new();
        readonly ReflectionData _reflectionData = new();
        readonly PreviousSectionData _previousSectionData = new();
        readonly FrameSlicingData _frameSlicingData = new();

        readonly ControllerNavigationLogic _controllerNavigationLogic;
        readonly DebugPanelViewLogic _debugPanelViewLogic;
        readonly DebugActionsWidgetsLogic _debugActionsWidgetsLogic;
        readonly DebugActionsSectionViewsLogic _debugActionsSectionViewsLogic;
        readonly DebugActionsSectionsLogic _debugActionsSectionsLogic;
        readonly PopupsLogic _popupsLogic;
        readonly SearchingLogic _searchingLogic;
        readonly ReflectionGenerationLogic _reflectionGenerationLogic;
        readonly ScrollingLogic _scrollingLogic;
        readonly ExtraPopupsLogic _extraPopupsLogic;
        readonly InputLogic _inputLogic;
        readonly PreviousSectionLogic _previousSectionLogic;
        readonly FrameSlicingLogic _frameSlicingLogic;

        readonly IPopupContainer _popupContainer;
        
        UDebugPanel()
        {
            _frameSlicingLogic = new FrameSlicingLogic(
                _frameSlicingData
            );
            
            _controllerNavigationLogic = new ControllerNavigationLogic(
                _controllersData,
                _debugActionsData,
                _debugPanelData
            );
            
            _previousSectionLogic = new PreviousSectionLogic(
                _previousSectionData,
                _debugPanelData
            );

            LoadingLogic loadingLogic = new(
                _debugPanelData
            );
            
            _debugActionsWidgetsLogic = new DebugActionsWidgetsLogic(
                _debugPanelData,
                _debugActionsData,
                _debugActionsPrefabsData,
                _controllerNavigationLogic
            );

            _debugActionsSectionViewsLogic = new DebugActionsSectionViewsLogic(
                _debugPanelData,
                _debugActionsData,
                _frameSlicingData,
                _debugActionsWidgetsLogic,
                loadingLogic
            );
            
            _popupsLogic = new PopupsLogic(
                _debugPanelData,
                _popupsData,
                _controllersData,
                _controllerNavigationLogic
            );

            _debugActionsSectionsLogic = new DebugActionsSectionsLogic(
                _debugPanelData,
                _debugActionsData,
                _debugActionsWidgetsLogic,
                _debugActionsSectionViewsLogic,
                loadingLogic,
                _previousSectionLogic
            );

            _searchingLogic = new SearchingLogic(
                _debugPanelData,
                _debugActionsData
            );

            _reflectionGenerationLogic = new ReflectionGenerationLogic(
                _debugActionsSectionsLogic,
                _reflectionData,
                _frameSlicingData
            );
            
            _extraPopupsLogic = new ExtraPopupsLogic(
                _debugPanelData,
                _debugActionsSectionsLogic,
                _popupsLogic
            );
            
            _debugPanelViewLogic = new DebugPanelViewLogic(
                _debugPanelData,
                _debugActionsWidgetsLogic,
                _controllerNavigationLogic,
                _extraPopupsLogic,
                _previousSectionLogic
            );

            _scrollingLogic = new ScrollingLogic(
                _debugPanelData
            );

            _inputLogic = new InputLogic(
                _inputData
            );
            
            _debugActionsSectionViewsLogic.Init(_debugActionsSectionsLogic);
            _previousSectionLogic.Init(_debugActionsSectionsLogic);

            _popupContainer = new PopupContainer(_popupsLogic);
        }

        /// <summary>
        /// Links a widget prefab with their respective debug action.
        /// This widget is automatically instantiated when the debug panel is shown
        /// and the action is present.
        /// </summary>
        public static void RegisterWidgetPrefab<T>(DebugActionWidget prefab) where T : IDebugAction
        {
            Type type = typeof(T);
            Instance._debugActionsPrefabsData.DebugActionTypeByDebugActionWidgetPrefabs[type] = prefab;
        }

        /// <summary>
        /// Automatically sets up input toggles for the
        /// debug panel using key inputs and touch gestures.
        /// </summary>
        /// <param name="keyCode">The key used to toggle the debug panel.</param>
        /// <param name="enableMiddleMouseButtonToggle">If true, the middle
        /// mouse button will toggle the debug panel.</param>
        /// <param name="enableThreeFingerTapToggle">If true, a three-finger
        /// tap gesture will toggle the debug panel (useful for mobile).</param>
        public static void SetupToggleInput(
            KeyCode keyCode = KeyCode.F1, 
            bool enableMiddleMouseButtonToggle = true, 
            bool enableThreeFingerTapToggle = true
            )
            => Instance.SetupToggleInputInternal(keyCode, enableMiddleMouseButtonToggle, enableThreeFingerTapToggle);

        /// <summary>
        /// Enables/disables navigation support for controllers.
        /// Must be set before showing the panel to take effect.
        /// Disabled by default.
        /// </summary>
        public static void SetControllerSupport(bool enable)
            => Instance.SetControllerSupportInternal(enable);

        /// <summary>
        /// Returns true if controller support has been enabled
        /// through the SetControllerSupport method.
        /// </summary>
        public static bool HasControllerSupportEnabled()
            => Instance.HasControllerSupportEnabledInternal();

        /// <summary>
        /// Shows the debug panel Ui.
        /// </summary>
        public static void Show()
        {
            OnVisibilityStateChanged?.Invoke(true);
            Instance.ShowInternalAsync().FireTaskAndForget();
        }
        
        /// <summary>
        /// Hides the debug panel Ui.
        /// </summary>
        public static void Hide()
        {
            Instance.HideInternal();
            OnVisibilityStateChanged?.Invoke(false);
        }

        /// <summary>
        /// Toggled the debug panel Ui.
        /// </summary>
        public static void Toggle()
        {
            if (IsVisible())
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        /// Checks if the debug panel is currently visible.
        /// </summary>
        /// <returns>True if the UI is visible, false otherwise.</returns>
        public static bool IsVisible() => Instance.IsVisibleInternal();
        
        /// <summary>
        /// Checks if the debug panel GameObject is currently instantiated.
        /// </summary>
        public static bool IsInstantiated() => Instance.IsInstantiatedInternal();

        /// <summary>
        /// Sets the global scale factor for the panel Ui.
        /// </summary>
        /// <param name="scaleFactor">The scale factor to apply. A value of 1.0 means default size,
        /// while values greater or less than 1.0 scale the content up or down respectively.</param>
        public static void SetPanelScaleFactor(float scaleFactor) => Instance.SetPanelScaleFactorInternal(scaleFactor);
        
        /// <summary>
        /// Sets the global panel canvas sortOrder.
        /// </summary>
        /// <param name="sortOrder">The sort order to apply. By default, the panel uses the maximum value</param>
        public static void SetPanelSortOrder(int sortOrder) => Instance.SetPanelSortOrderInternal(sortOrder);
        
        /// <summary>
        /// Adds a section to the debug panel.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="priority">The priority of the section.</param>
        /// <returns>The added section.</returns>
        public static IDebugActionsSection AddSection(string name, int priority = 0) 
            => Instance.AddSectionInternal(name, priority);
        
        /// <summary>
        /// Adds a section to the debug panel with specified options object.
        /// Using reflection, debug actions are going to be created for the options object.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="optionsObject">Options associated with the section.</param>
        /// <param name="priority">The priority of the section.</param>
        /// <returns>The added section.</returns>
        public static IDebugActionsSection AddSection(string name, object optionsObject, int priority = 0) 
            => Instance.AddSectionInternal(name, optionsObject, priority);
        
        /// <summary>
        /// Using reflection, creates debug actions for each compatible property inside the class/struct.
        /// </summary>
        public static void AddOptionsObject(object optionsObject) 
            => Instance.AddOptionsObjectInternal(optionsObject);
        
        /// <summary>
        /// Searches for all the classes/structs that have the attribute DebugActionsObjectAttribute, and
        /// using reflection, creates debug actions for each compatible property inside the class/struct.
        /// </summary>
        public static void AddGlobalOptionsObjects() 
            => Instance.AddGlobalOptionsObjectsInternal();
        
        /// <summary>
        /// Searches for all the classes/structs that have the attribute DebugActionsObjectAttribute, and
        /// using reflection, creates debug actions for each compatible property inside the class/struct.
        /// This Async method distributes the calculations between different frames, to not clog the current
        /// frame.
        /// </summary>
        public static Task AddGlobalOptionsObjectsAsync(CancellationToken cancellationToken = default) 
            => Instance.AddGlobalOptionsObjectsInternalAsync(cancellationToken);
        
        /// <summary>
        /// Removes a section from the debug panel.
        /// </summary>
        /// <param name="section">The section to be removed.</param>
        public static void RemoveSection(IDebugActionsSection section) 
            => Instance.RemoveSectionInternal(section);

        /// <summary>
        /// Sets up a button that's located on the top right of the debug panel,
        /// with the provied action. Useful for quickly executing some functionality
        /// from everywhere on the panel.
        /// </summary>
        public static void SetShortcutAction(string name, Action action)
            => Instance.SetShortcutActionInternal(name, action);
        
        [Obsolete("Use UDebugPanel.Popups.AddPopup instead")]
        public static void AddPopup(Func<Transform, GameObject> spawnPopupAction)
            => Popups.AddPopup(spawnPopupAction);
        
        [Obsolete("Use UDebugPanel.Popups.RemovePopup instead")]
        public static void RemovePopup(GameObject popup)
            => Popups.RemovePopup(popup);
        
        [Obsolete("Use UDebugPanel.Popups.RemoveCurrentPopup instead")]
        public static void RemoveCurrentPopup()
            => Popups.RemoveCurrentPopup();
        
        public static void CoreTrySelectSectionWithMorePriority()
            => Instance.TrySelectSectionWithMorePirorityInternal();
        
        public static void CoreTickToggleInput()
            => Instance.TickToggleInputInternal();
        
        public static void CoreSetFrameSlicingMaxMs(int maxMs)
            => Instance.SetFrameSlicingMaxMsInternal(maxMs);

        public static DebugActionWidget? CoreCreateDebugActionWidget(
            DebugAction debugAction,
            RectTransform parent
        )
        {
            if (_instance == null)
            {
                return null;
            }
            
            return _instance._debugActionsWidgetsLogic.CreateDebugActionWidget(
                debugAction,
                parent
            );
        }

        void SetupToggleInputInternal(
            KeyCode keyCode = KeyCode.F1, 
            bool enableMiddleMouseButtonToggle = true, 
            bool enableThreeFingerTapToggle = true
            )
        {
            _inputLogic.SetupToggleInput(
                keyCode,
                enableMiddleMouseButtonToggle,
                enableThreeFingerTapToggle
            );
        }

        void SetControllerSupportInternal(bool set)
        {
            _controllerNavigationLogic.SetControllerSupport(set);
        }

        bool HasControllerSupportEnabledInternal()
        {
            return _controllerNavigationLogic.HasControllerSupportEnabled();
        }
        
        async Task ShowInternalAsync()
        {
            _popupsLogic.ClearPopupsStack();
            
            await _debugPanelViewLogic.InstantiateDebugPanelView();
            
            _searchingLogic.ConnectSearchText();
            _searchingLogic.ConnectClearSearchText();

            await _debugActionsSectionViewsLogic.InstantiateAllSectionViewsAsync();

            _previousSectionLogic.LoadPreviousSessionSection();
            _previousSectionLogic.RefreshPreviousSectionButtonWhenSectionChanges();
            
            DebugActionsSection? section = _debugActionsSectionsLogic.GetSelectedSection();

            if (section != null)
            {
                await _debugActionsSectionsLogic.SetSelectedSectionAsync(
                    section,
                    !_debugPanelData.IsUsingLastSectionAdded
                );
            }
            else
            {
                await _debugActionsSectionsLogic.TrySelectSectionWithMorePriorityAsync();
            }
            
            if (!string.IsNullOrEmpty(_debugPanelData.SearchFilter))
            {
                _searchingLogic.RefreshWidgetVisibilityBySearchText();
            }
            
            _scrollingLogic.ResetScrollPosition();
            
            _controllerNavigationLogic.SelectFirstSectionWidget(section);
        }
        
        void HideInternal()
        {
            _debugActionsSectionViewsLogic.ClearDebugActionsSectionViewsLists();
            _debugActionsWidgetsLogic.ClearDebugActionsWidgetsLists();
            _debugPanelViewLogic.DestroyDebugPanelView();
        }
        
        bool IsVisibleInternal()
        {
            return _debugPanelViewLogic.IsVisible();
        }
        
        bool IsInstantiatedInternal()
        {
            return _debugPanelViewLogic.IsInstantiated();
        }

        void SetPanelScaleFactorInternal(float scaleFactor)
        {
            _debugPanelViewLogic.SetScaleFactor(scaleFactor);
        }
        
        void SetPanelSortOrderInternal(int sortOrder)
        {
            _debugPanelViewLogic.SetSortOrder(sortOrder);
        }

        IDebugActionsSection AddSectionInternal(string name, int priority)
        {
            return _debugActionsSectionsLogic.AddDebugActionsSection(
                name,
                priority
            );
        }
        
        IDebugActionsSection AddSectionInternal(string name, object optionsObject, int priority)
        {
            DebugActionsSection section = _debugActionsSectionsLogic.AddDebugActionsSection(
                name,
                priority
            );
            
            _reflectionGenerationLogic.PopulateDebugActionsSectionFromOptionsObject(section, optionsObject);
            
            return section;
        }
        
        void AddOptionsObjectInternal(object optionsObject)
        {
            _reflectionGenerationLogic.PopulateDebugActionsFromOptionsObject(optionsObject);
        }

        void AddGlobalOptionsObjectsInternal()
        {
            _reflectionGenerationLogic.PopulateDebugActionsFromGlobalOptionsObjects();
        }
        
        Task AddGlobalOptionsObjectsInternalAsync(CancellationToken cancellationToken)
        {
            return _reflectionGenerationLogic.PopulateDebugActionsFromGlobalOptionsObjectsAsync(cancellationToken);
        }
        
        void RemoveSectionInternal(IDebugActionsSection section)
        {
            _debugActionsSectionsLogic.RemoveDebugActionsSection(section);
        }

        void SetShortcutActionInternal(string name, Action action)
        {
            _debugPanelViewLogic.SetShortcutAction(name, action);
        }

        void AddPopupInternal(Func<Transform, GameObject> spawnPopupAction)
        {
            _popupsLogic.AddPopup(spawnPopupAction);
        }
        
        void RemovePopupInternal(GameObject popup)
        {
            _popupsLogic.RemovePopup(popup);
        }
        
        void RemoveCurrentPopupInternal()
        {
            _popupsLogic.RemoveCurrentPopup();
        }

        void TrySelectSectionWithMorePirorityInternal()
        {
            _debugActionsSectionsLogic.TrySelectSectionWithMorePirorityIfSectionsChanged();
        }

        void TickToggleInputInternal()
        {
            _inputLogic.TickToggleInput();
        }

        void SetFrameSlicingMaxMsInternal(int maxMs)
        {
            _frameSlicingLogic.SetMaxMsPerFrame(maxMs);
        }
    }   
}
