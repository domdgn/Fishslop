#nullable enable
using System.IO;
using DebugPanel.Constants;
using UnityEditor;

namespace DebugPanel.Utils
{
    public static class UDebugPanelBuildUtils
    {
        public static void StripFromBuild()
        {
            string? assetPath = GetResourcesPrefabFilepath(false);
            string metaPath = $"{assetPath}.meta";

            if (string.IsNullOrEmpty(assetPath))
            {
                UnityEngine.Debug.LogError($"Failed to find UDebugPanel asset to strip");
                return;
            }

            bool exists = File.Exists(assetPath);

            if (!exists)
            {
                UnityEngine.Debug.LogError("Cannot strip UDebugPanel from build because file was not found");
                return;
            }

            string? newPath = GetResourcesPrefabFilepath(true);
            string newMetaPath = $"{newPath}.meta";
            
            if (string.IsNullOrEmpty(newPath))
            {
                UnityEngine.Debug.LogError("Failed to strip UDebugPanel asset");
                return;
            }
            
            File.Move(assetPath, newPath);
            File.Move(metaPath, newMetaPath);
            
            AssetDatabase.Refresh();
        }

        public static void UnstripFromBuild()
        {
            string? assetPath = GetResourcesPrefabFilepath(true);
            string metaPath = $"{assetPath}.meta";

            if (string.IsNullOrEmpty(assetPath))
            {
                UnityEngine.Debug.LogError($"Failed to find UDebugPanel asset to unstrip");
                return;
            }
            
            bool exists = File.Exists(assetPath);

            if (!exists)
            {
                UnityEngine.Debug.LogError("Cannot strip UDebugPanel from build because file was not found");
                return;
            }

            string? newPath = GetResourcesPrefabFilepath(false);
            string newMetaPath = $"{newPath}.meta";
            
            if (string.IsNullOrEmpty(newPath))
            {
                UnityEngine.Debug.LogError($"Failed to strip UDebugPanel asset");
                return;
            }
            
            File.Move(assetPath, newPath);
            File.Move(metaPath, newMetaPath);
            
            AssetDatabase.Refresh();
        }

        public static bool IsStrippedFromBuild()
        {
            string? assetPath = GetResourcesPrefabFilepath(false);
            return File.Exists(assetPath);
        }

        public static string? GetResourcesPrefabFilepath(bool stripped)
        {
            string? rootFolder = GetRootFolder();
            
            if (string.IsNullOrEmpty(rootFolder))
            {
                return null;
            }
            
            string resourcesPath = $"{rootFolder}{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}";
            
            string strippedChar = stripped ? "~" : "";
            string directoryName = $"{resourcesPath}{strippedChar}{ResourcesConstants.ResourcesFilename}.prefab";
            
            return directoryName;
        }
        
        public static string? GetRootFolder()
        {
            string? logoPath = UDebugPanelEditorUtils.FindAssetPathByAssetName("DebugPanelLogo");
            
            if (string.IsNullOrEmpty(logoPath))
            {
                return null;
            }
            
            string? directoryName = Path.GetDirectoryName(logoPath);

            if (string.IsNullOrEmpty(directoryName))
            {
                return null;
            }
            
            DirectoryInfo? rootDirectoryInfo = Directory.GetParent(directoryName);

            return rootDirectoryInfo?.Parent?.FullName;
        }
    }
}