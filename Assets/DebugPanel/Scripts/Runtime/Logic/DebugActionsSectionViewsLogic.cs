#nullable enable

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DebugPanel.Constants;
using DebugPanel.Datas;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Containers;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Views;
using UnityEngine;

namespace DebugPanel.Logic
{
    public sealed class DebugActionsSectionViewsLogic
    {
        readonly DebugPanelData _debugPanelData;
        readonly DebugActionsData _debugActionsData;
        readonly FrameSlicingData _frameSlicingData;
        readonly DebugActionsWidgetsLogic _debugActionsWidgetsLogic;
        readonly LoadingLogic _loadingLogic;
        
        DebugActionsSectionsLogic? _debugActionsSectionsLogic;

        public DebugActionsSectionViewsLogic(
            DebugPanelData debugPanelData, 
            DebugActionsData debugActionsData, 
            FrameSlicingData frameSlicingData,
            DebugActionsWidgetsLogic debugActionsWidgetsLogic, 
            LoadingLogic loadingLogic)
        {
            _debugPanelData = debugPanelData;
            _debugActionsData = debugActionsData;
            _frameSlicingData = frameSlicingData;
            _debugActionsWidgetsLogic = debugActionsWidgetsLogic;
            _loadingLogic = loadingLogic;
        }

        public void Init(DebugActionsSectionsLogic debugActionsSectionsLogic)
        {
            _debugActionsSectionsLogic = debugActionsSectionsLogic;
        }

        public void ClearDebugActionsSectionViewsLists()
        {
            _debugActionsData.SectionsViews.Clear();
        }

        public void InstantiateSectionView(DebugActionsSection section)
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }
            
            bool alreadyExists = _debugActionsData.SectionsViews.ContainsKey(section);

            if (alreadyExists)
            {
                return;
            }

            DebugPanelView debugPanelView = _debugPanelData.DebugPanelView;

            DebugPanelSectionView instance = Object.Instantiate(
                debugPanelView.DebugPanelSectionViewPrefab,
                debugPanelView.SectionsContentParent
            );

            void Select() => _debugActionsSectionsLogic!.SetSelectedSection(section, true);
            
            instance.TitleLabel.text = section.Name;
            instance.Button.onClick.AddListener(Select);
            
            _debugActionsData.SectionsViews.Add(section, instance);
        }

        public async Task<bool> InstantiateAllSectionViewsAsync()
        {
            _loadingLogic.SetSectionsLoading(true);
            
            Stopwatch frameTimeStopwatch = Stopwatch.StartNew();

            List<DebugActionsSection> sectionsToSpawn = new(_debugActionsData.Sections);

            while (sectionsToSpawn.Count > 0)
            {
                DebugActionsSection section = sectionsToSpawn[0];
                sectionsToSpawn.RemoveAt(0);
                
                bool stillExists = _debugActionsData.Sections.Contains(section);

                if (!stillExists)
                {
                    continue;
                }
                
                InstantiateSectionView(section);
                
                if (frameTimeStopwatch.ElapsedMilliseconds > _frameSlicingData.MaxMsPerFrame)
                {
                    await Task.Yield();

                    frameTimeStopwatch.Restart();
                    
                    if (_debugPanelData.DebugPanelView == null)
                    {
                        return false;
                    }   
                }
            }
            
            ReorderSectionViewsByPriority();

            _loadingLogic.SetSectionsLoading(false);
            
            return true;
        }
        
        public async Task<bool> InstantiateSelectedSectionDebugActionsWidgetsAsync(DebugActionsSection section)
        {
            Stopwatch frameTimeStopwatch = Stopwatch.StartNew();
            
            for (int y = 0; y < section.DebugActions.Count; y++)
            {
                if (_debugPanelData.SelectedSection != section)
                {
                    return false;
                }
                
                IDebugAction debugAction = section.DebugActions[y];
                    
                _debugActionsWidgetsLogic.CreateDebugActionWidgetOnMainPanel(
                    debugAction
                );
                
                if (frameTimeStopwatch.ElapsedMilliseconds > _frameSlicingData.MaxMsPerFrame)
                {
                    await Task.Yield();

                    frameTimeStopwatch.Restart();
                    
                    if (_debugPanelData.DebugPanelView == null)
                    {
                        return false;
                    }   
                }
            }
            
            return true;
        }

        public void DestroySectionView(IDebugActionsSection section)
        {
            if (!(section is DebugActionsSection debugActionsSection))
            {
                return;
            }
            
            debugActionsSection.Dispose();

            bool viewFound = _debugActionsData.SectionsViews.TryGetValue(
                debugActionsSection,
                out DebugPanelSectionView sectionView
            );

            if (!viewFound)
            {
                return;
            }

            _debugActionsData.SectionsViews.Remove(debugActionsSection);

            if (sectionView != null)
            {
                Object.Destroy(sectionView!.gameObject);
            }   
        }
        
        public void RefreshSectionWidgets(DebugActionsSection section)
        {
            foreach (IDebugAction action in section.DebugActions)
            {
                _debugActionsWidgetsLogic.RefreshDebugActionWidgetInstanceLife(action);
                
                bool viewFound = _debugActionsData.Widgets.TryGetValue(
                    action,
                    out DebugActionWidget? widget
                );

                if (!viewFound)
                {
                    continue;
                }
                
                widget!.Refresh();
            }
        }

        public void RefreshSelectedSectionDocumentationButtonVisibility()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            if (_debugPanelData.SelectedSection == null)
            {
                return;
            }

            DebugActionsSection section = _debugPanelData.SelectedSection;

            bool hasAny = false;
            
            foreach (IDebugAction debugAction in section.DebugActions)
            {
                bool hasDocumentation = !string.IsNullOrEmpty(debugAction.Documentation);

                if (!hasDocumentation)
                {
                    continue;
                }

                hasAny = true;
                break;
            }
            
            _debugPanelData.DebugPanelView.SectionDocumentationButton.gameObject.SetActive(hasAny);
        }

        public void ReorderSectionViewsByPriority()
        {
            IOrderedEnumerable<DebugActionsSection> orderedSections = _debugActionsSectionsLogic!.GetOrderedSectionsByPriority();

            int index = 0;
            foreach (DebugActionsSection debugActionsSection in orderedSections)
            {
                bool viewFound = _debugActionsData.SectionsViews.TryGetValue(
                    debugActionsSection,
                    out DebugPanelSectionView debugPanelSectionView
                );

                if (!viewFound)
                {
                    continue;
                }

                // This can happen when stopping unity from the editor
                if (debugPanelSectionView == null)
                {
                    return;
                }

                debugPanelSectionView!.transform.SetSiblingIndex(index);
                ++index;
            }
        }

        public void SetSectionViewVisuallySelected(DebugActionsSection? section, bool selected)
        {
            if (section == null)
            {
                return;
            }
            
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            DebugPanelView debugPanelView = _debugPanelData.DebugPanelView;
            
            bool viewFound = _debugActionsData.SectionsViews.TryGetValue(
                section,
                out DebugPanelSectionView sectionView
            );

            if (!viewFound)
            {
                return;
            }

            sectionView!.BackgroundImage.color = selected ? debugPanelView.SelectedSectionColor : debugPanelView.BaseSectionColor;
        }
    }
}