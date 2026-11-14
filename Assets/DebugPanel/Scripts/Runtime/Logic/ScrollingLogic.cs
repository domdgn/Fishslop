using DebugPanel.Datas;
using UnityEngine;

namespace DebugPanel.Logic
{
    public sealed class ScrollingLogic
    {
        readonly DebugPanelData _debugPanelData;

        public ScrollingLogic(DebugPanelData debugPanelData)
        {
            _debugPanelData = debugPanelData;
        }

        public void ResetScrollPosition()
        {
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }
            
            _debugPanelData.DebugPanelView.SectionsContentParent.anchoredPosition = new Vector2(
                _debugPanelData.DebugPanelView.SectionsContentParent.anchoredPosition.x,
                _debugPanelData.SectionsScrollPosition
            );

            _debugPanelData.DebugPanelView.WidgetsContentParent.anchoredPosition = new Vector2(
                _debugPanelData.DebugPanelView.WidgetsContentParent.anchoredPosition.x,
                _debugPanelData.WidgetsScrollPosition
            );
        }
    }
}