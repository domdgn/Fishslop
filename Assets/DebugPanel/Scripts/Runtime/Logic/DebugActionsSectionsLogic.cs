#nullable enable

using System.Linq;
using System.Threading.Tasks;
using DebugPanel.Datas;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;

namespace DebugPanel.Logic
{
    public sealed class DebugActionsSectionsLogic
    {
        readonly DebugPanelData _debugPanelData;
        readonly DebugActionsData _debugActionsData;
        readonly DebugActionsWidgetsLogic _debugActionsWidgetsLogic;
        readonly DebugActionsSectionViewsLogic _debugActionsSectionViewsLogic;
        readonly LoadingLogic _loadingLogic;
        readonly PreviousSectionLogic _previousSectionLogic;
        
        public DebugActionsSectionsLogic(
            DebugPanelData debugPanelData, 
            DebugActionsData debugActionsData, 
            DebugActionsWidgetsLogic debugActionsWidgetsLogic, 
            DebugActionsSectionViewsLogic debugActionsSectionViewsLogic, 
            LoadingLogic loadingLogic, 
            PreviousSectionLogic previousSectionLogic
            )
        {
            _debugPanelData = debugPanelData;
            _debugActionsData = debugActionsData;
            _debugActionsWidgetsLogic = debugActionsWidgetsLogic;
            _debugActionsSectionViewsLogic = debugActionsSectionViewsLogic;
            _loadingLogic = loadingLogic;
            _previousSectionLogic = previousSectionLogic;
        }

        public DebugActionsSection AddDebugActionsSection(
            string name,
            int priority
        )
        {
            DebugActionsSection section = new DebugActionsSection(
                name,
                priority,
                _debugActionsSectionViewsLogic.RefreshSectionWidgets
            );

            section.OnAdded += _debugActionsWidgetsLogic.CreateDebugActionWidgetOnMainPanelIfFromSelectedSection;
            section.OnRemoved += _debugActionsWidgetsLogic.DestroyMainPanelDebugActionWidget;
            
            _debugActionsData.Sections.Add(section);

            _debugActionsSectionViewsLogic.InstantiateSectionView(section);
            _debugActionsSectionViewsLogic.ReorderSectionViewsByPriority();

            _debugPanelData.ShouldTryToFindSelectedSection = true;

            _previousSectionLogic.RefreshPreviousSectionButtonWhenSectionChanges();
            
            return section;
        }
        
        public DebugActionsSection? GetDebugActionsSection(string name)
        {
            foreach (DebugActionsSection checkingSection in _debugActionsData.Sections)
            {
                if (string.Equals(checkingSection.Name, name))
                {
                    return checkingSection;
                }
            }

            return null;
        }

        public DebugActionsSection GetOrAddDebugActionsSection(string name, int priority)
        {
            DebugActionsSection? checkingSection = GetDebugActionsSection(name);

            if (checkingSection != null)
            {
                return checkingSection;
            }

            return AddDebugActionsSection(name, priority);
        }

        public void RemoveDebugActionsSection(IDebugActionsSection section)
        {
            if (!(section is DebugActionsSection debugActionsSection))
            {
                return;
            }

            bool found = _debugActionsData.Sections.Remove(debugActionsSection);

            if (!found)
            {
                return;
            }
            
            foreach (IDebugAction debugAction in debugActionsSection.DebugActions)
            {
                _debugActionsWidgetsLogic.DestroyMainPanelDebugActionWidget(debugAction);
                
                if (debugAction is DebugAction castedDebugAction)
                {
                    castedDebugAction.WhenRemoved?.Invoke();
                }
            }

            debugActionsSection.Dispose();

            TryDeselectSection(debugActionsSection);

            _previousSectionLogic.RefreshPreviousSectionButtonWhenSectionChanges();
            
            _debugActionsSectionViewsLogic.DestroySectionView(debugActionsSection);

            _debugPanelData.ShouldTryToFindSelectedSection = true;
        }

        public IOrderedEnumerable<DebugActionsSection> GetOrderedSectionsByPriority()
        {
            return _debugActionsData.Sections.OrderByDescending(o => o.Priority);
        }

        public DebugActionsSection? GetSelectedSection()
        {
            if (_debugPanelData.SelectedSection != null)
            {
                return _debugPanelData.SelectedSection!;
            }

            _debugPanelData.SelectedSection = GetOrderedSectionsByPriority().FirstOrDefault();
            
            return _debugPanelData.SelectedSection;
        }

        public void SetSelectedSection(DebugActionsSection section, bool selectedByTheUser)
        {
            SetSelectedSectionAsync(section, selectedByTheUser).FireTaskAndForget();
        }

        public async Task SetSelectedSectionAsync(DebugActionsSection section, bool selectedByTheUser)
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }
            
            _loadingLogic.SetWidgetsLoading(true);
            
            _debugActionsWidgetsLogic.DestroyAllMainPanelDebugActionWidgets();
            _debugActionsSectionViewsLogic.SetSectionViewVisuallySelected(_debugPanelData.SelectedSection, false);
            
            _debugPanelData.SelectedSection = section;
            _debugPanelData.IsUsingLastSectionAdded = !selectedByTheUser;
            
            if (selectedByTheUser)
            {
                _previousSectionLogic.SetPreviousSection(_debugPanelData.SelectedSection!);
            }
            
            _previousSectionLogic.RefreshPreviousSectionButtonWhenSectionChanges();
            
            _debugActionsSectionViewsLogic.SetSectionViewVisuallySelected(_debugPanelData.SelectedSection, true);
            
            _debugActionsSectionViewsLogic.RefreshSelectedSectionDocumentationButtonVisibility();
            
            string sectionName = _debugPanelData.SelectedSection != null ? _debugPanelData.SelectedSection.Name : string.Empty;
            _debugPanelData.DebugPanelView.WidgetsSelectedSectionLabel.text = sectionName;
            
            bool success = await _debugActionsSectionViewsLogic.InstantiateSelectedSectionDebugActionsWidgetsAsync(section!);

            if (!success)
            {
                return;
            }
            
            _loadingLogic.SetWidgetsLoading(false);
        }

        public void TryDeselectSection(DebugActionsSection debugActionsSection)
        {
            bool isSelectedSection = _debugPanelData.SelectedSection == debugActionsSection;

            if (isSelectedSection)
            {
                _debugActionsSectionViewsLogic.SetSectionViewVisuallySelected(_debugPanelData.SelectedSection, false);
 
                _debugPanelData.SelectedSection = null;
                _debugPanelData.IsUsingLastSectionAdded = true;
            }

            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            _debugPanelData.DebugPanelView.WidgetsSelectedSectionLabel.text = "";
        }

        public void TrySelectSectionWithMorePriority()
        {
            TrySelectSectionWithMorePriorityAsync().FireTaskAndForget();
        }
        
        public async Task TrySelectSectionWithMorePriorityAsync()
        {
            if (!_debugPanelData.IsUsingLastSectionAdded)
            {
                return;
            }

            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            if (_debugPanelData.SelectedSection != null)
            {
                TryDeselectSection(_debugPanelData.SelectedSection);
            }
            
            DebugActionsSection? section = GetSelectedSection();

            if (section != null)
            {
                await SetSelectedSectionAsync(section, false);
            }
        }

        public void TrySelectSectionWithMorePirorityIfSectionsChanged()
        {
            if (!_debugPanelData.ShouldTryToFindSelectedSection)
            {
                return;
            }

            _debugPanelData.ShouldTryToFindSelectedSection = false;

            TrySelectSectionWithMorePriority();
        }
    }
}