using DebugPanel.Constants;
using DebugPanel.Utils;
using UnityEditor;

namespace SRDebugger.Editor
{
    public static class DevelopmentMenuItems
    {
#if UDEBUG_PANEL_DEVELOPMENT
        [MenuItem(EditorPathsConstants.DevelopmentStripFromBuildItem)]
        public static void StripFromBuild()
        {
            UDebugPanelBuildUtils.StripFromBuild();
        }
        
        [MenuItem(EditorPathsConstants.DevelopmentUnstripFromBuildItem)]
        public static void UnstripFromBuild()
        {
            UDebugPanelBuildUtils.UnstripFromBuild();
        }
#endif
    }
}