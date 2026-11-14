#nullable enable
using DebugPanel.Constants;
using DebugPanel.Datas;
using DebugPanel.DebugActions.Containers;
using UnityEngine;

namespace DebugPanel.Logic
{
    public sealed class PreviousSectionLogic
    {
        readonly PreviousSectionData _previousSectionData;
        readonly DebugPanelData _debugPanelData;

        DebugActionsSectionsLogic? _debugActionsSectionsLogic;

        public PreviousSectionLogic(
            PreviousSectionData previousSectionData, 
            DebugPanelData debugPanelData
            )
        {
            _previousSectionData = previousSectionData;
            _debugPanelData = debugPanelData;
        }

        public void Init(DebugActionsSectionsLogic debugActionsSectionsLogic)
        {
            _debugActionsSectionsLogic = debugActionsSectionsLogic;
        }

        public void LoadPreviousSessionSection()
        {
            _previousSectionData.PreviousSectionName = PlayerPrefs.GetString(
                PlayerPrefsConstants.PreviousSectionNamePrefsKey
            );
        }

        public void SaveSessionSection()
        {
            bool isValid = !string.IsNullOrEmpty(_previousSectionData.PreviousSectionName);

            if (!isValid)
            {
                return;
            }

            PlayerPrefs.SetString(
                PlayerPrefsConstants.PreviousSectionNamePrefsKey,
                _previousSectionData.PreviousSectionName
            );
            
            PlayerPrefs.Save();
        }

        public bool IsPreviousSectionName(DebugActionsSection debugActionsSection)
        {
            return string.Equals(_previousSectionData.PreviousSectionName, debugActionsSection.Name);
        }
        
        public void SetPreviousSection(DebugActionsSection debugActionsSection)
        {
            _previousSectionData.PreviousSectionName = debugActionsSection.Name;

            SaveSessionSection();
        }

        public void RefreshPreviousSectionButtonWhenSectionChanges()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }
            
            string previousSectionName = _previousSectionData.PreviousSectionName ?? string.Empty;
            
            DebugActionsSection? section = _debugActionsSectionsLogic!.GetDebugActionsSection(previousSectionName);

            bool previousSectionExists = section != null;
            bool previousSectionIsCurrentSection = section == _debugPanelData.SelectedSection;

            bool shouldShowPreviousSectionContent = previousSectionExists && !previousSectionIsCurrentSection;
            
            _debugPanelData.DebugPanelView.PreviousSectionContent.gameObject.SetActive(shouldShowPreviousSectionContent);
            
            if (!previousSectionExists)
            {
                return;
            }

            _debugPanelData.DebugPanelView.PreviousSectionButtonLabel.text = section!.Name;
        }

        public void SelectPreviousSection()
        {
            if (string.IsNullOrEmpty(_previousSectionData.PreviousSectionName))
            {
                return;
            }

            DebugActionsSection? section = _debugActionsSectionsLogic!.GetDebugActionsSection(_previousSectionData.PreviousSectionName);

            if (section == null)
            {
                return;
            }
            
            _debugActionsSectionsLogic.SetSelectedSection(section, true);
            RefreshPreviousSectionButtonWhenSectionChanges();
        }
    }
}