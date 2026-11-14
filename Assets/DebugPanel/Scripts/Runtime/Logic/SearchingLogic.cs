using System.Collections.Generic;
using DebugPanel.Datas;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Extensions;

namespace DebugPanel.Logic
{
    public sealed class SearchingLogic
    {
        readonly DebugPanelData _debugPanelData; 
        readonly DebugActionsData _debugActionsData;

        public SearchingLogic(
            DebugPanelData debugPanelData, 
            DebugActionsData debugActionsData
            )
        {
            _debugPanelData = debugPanelData;
            _debugActionsData = debugActionsData;
        }
        
        public void ConnectClearSearchText()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }
        
            void Clear()
            {
                _debugPanelData.DebugPanelView.FilterInputField.text = string.Empty;
                _debugPanelData.SearchFilter = string.Empty;
                RefreshWidgetVisibilityBySearchText();
            }
        
            _debugPanelData.DebugPanelView.ClearFilterButton.onClick.AddListener(Clear);
        }

        public void ConnectSearchText()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            void Refresh(string text) => RefreshWidgetVisibilityBySearchText();
        
            _debugPanelData.DebugPanelView.FilterInputField.text = _debugPanelData.SearchFilter;
            _debugPanelData.DebugPanelView.FilterInputField.onValueChanged.AddListener(Refresh);
        }

        public void RefreshWidgetVisibilityBySearchText()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            _debugPanelData.SearchFilter = _debugPanelData.DebugPanelView.FilterInputField.text;
            string searchFilter = _debugPanelData.SearchFilter.ToLowerNoSpaces();

            string[] parts = searchFilter.Split(':');
            string filterMode = parts.Length == 2 ? parts[0] : string.Empty;
            searchFilter = parts.Length == 2 ? parts[1] : searchFilter;

            foreach (KeyValuePair<IDebugAction, DebugActionWidget> entry in _debugActionsData.Widgets)
            {
                bool visible = ShouldDebugActionBeVisibleWithSerachField(
                    filterMode,
                    searchFilter,
                    entry.Key
                );

                entry.Value!.gameObject.SetActive(visible);
            }
        }

        public void SetSearchText(string text)
        {
            _debugPanelData.SearchFilter = text;

            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            _debugPanelData.DebugPanelView.FilterInputField.text = text;
        }

        public bool ShouldDebugActionBeVisibleWithSerachField(string filterMode, string searchFilter, IDebugAction debugAction)
        {
            if (string.IsNullOrEmpty(searchFilter))
            {
                return true;
            }

            string normalizedString = debugAction.ActionName.ToLowerNoSpaces();
            
            return normalizedString.Contains(searchFilter);
        }
    }
}