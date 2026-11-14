#nullable enable

using UnityEditor;

namespace DebugPanel.Utils
{
    public static class UDebugPanelEditorUtils
    {
        public static string? FindAssetPathByAssetName(string name) 
        {
            string[] guids = AssetDatabase.FindAssets($"t:Object {name}");

            if (guids.Length == 0)
            {
                return null;
            }
            
            return AssetDatabase.GUIDToAssetPath(guids[0]);
        }
    }
}