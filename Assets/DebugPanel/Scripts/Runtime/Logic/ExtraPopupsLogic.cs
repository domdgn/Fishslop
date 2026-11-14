using System.Collections.Generic;
using System.Linq;
using System.Text;
using DebugPanel.Datas;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Views;

namespace DebugPanel.Logic
{
    public sealed class ExtraPopupsLogic
    {
        readonly DebugPanelData _debugPanelData;
        readonly DebugActionsSectionsLogic _debugActionsSectionsLogic;
        readonly PopupsLogic _popupsLogic;

        public ExtraPopupsLogic(
            DebugPanelData debugPanelData, 
            DebugActionsSectionsLogic debugActionsSectionsLogic, 
            PopupsLogic popupsLogic
            )
        {
            _debugPanelData = debugPanelData;
            _debugActionsSectionsLogic = debugActionsSectionsLogic;
            _popupsLogic = popupsLogic;
        }

        public void ShowDocumentationPopup()
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

            DebugPanelDocumentationPopup popup = _debugPanelData.DebugPanelView.DocumentationPopup;

            StringBuilder stringBuilder = new StringBuilder();

            foreach (IDebugAction debugAction in section.DebugActions)
            {
                bool hasDocumentation = !string.IsNullOrEmpty(debugAction.Documentation);

                if (!hasDocumentation)
                {
                    continue;
                }

                stringBuilder.AppendLine($"<b>-{debugAction.ActionName}:</b> {debugAction.Documentation}");
            }

            void OnClose() => _popupsLogic.RemovePopupFromStack(popup.gameObject);
            
            popup.Setup(
                $"{section.Name} Documentation",
                stringBuilder.ToString(),
                OnClose
            );
            
            popup.gameObject.SetActive(true);
            _popupsLogic.AddPopupToStack(popup.gameObject);
        }

        public void ShowSectionSelectorPopup()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }

            IOrderedEnumerable<DebugActionsSection> orderedSections =
                _debugActionsSectionsLogic.GetOrderedSectionsByPriority();
            
            IEnumerable<DebugActionsSection> sections = orderedSections.OrderBy(s => s.Name);

            void SetSelected(DebugActionsSection section) => _debugActionsSectionsLogic.SetSelectedSection(section, true);
            
            DebugPanelSectionSelectorPopup popup = _debugPanelData.DebugPanelView.SectionSelectorPopup;
            
            void OnClose() => _popupsLogic.RemovePopupFromStack(popup.gameObject);
            
            popup.Setup(sections, SetSelected, OnClose);
            
            popup.gameObject.SetActive(true);
            _popupsLogic.AddPopupToStack(popup.gameObject);
        }
    }
}