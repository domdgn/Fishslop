using DebugPanel.Datas;

namespace DebugPanel.Logic
{
    public sealed class LoadingLogic
    {
        readonly DebugPanelData _debugPanelData;

        public LoadingLogic(DebugPanelData debugPanelData)
        {
            _debugPanelData = debugPanelData;
        }

        public void SetSectionsLoading(bool loading)
        {
            _debugPanelData.IsLoadingSections = loading;
            
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }
            
            _debugPanelData.DebugPanelView.SectionsContent.SetActive(!loading);
        }

        public void SetWidgetsLoading(bool loading)
        {
            _debugPanelData.IsLoadingWidgets = loading;
            
            if (_debugPanelData.DebugPanelView == null)
            {
                return;
            }
            
            _debugPanelData.DebugPanelView.WidgetsContent.SetActive(!loading);
        }
    }
}