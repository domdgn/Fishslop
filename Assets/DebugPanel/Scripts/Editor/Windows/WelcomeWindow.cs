#nullable enable

using System.IO;
using DebugPanel.Constants;
using DebugPanel.Core;
using DebugPanel.Utils;
using UnityEditor;
using UnityEngine;

namespace SRDebugger.Editor
{
    [InitializeOnLoad]
    sealed class WelcomeWindow : EditorWindow
    {
        const string WelcomeWindowShownForVersionPlayerPrefsKey = "UDebugPanel.WelcomeWindow.ShownForVersion";
        
        static Texture2D? _welcomeLogoTexture;
        
        static WelcomeWindow()
        {
            EditorApplication.update += OpenUpdate;
        }

        static void OpenUpdate()
        {
            if (ShouldOpen())
            {
                Open();
            }

            EditorApplication.update -= OpenUpdate;
        }

        [MenuItem(EditorPathsConstants.WelcomeWindowItemPath)]
        public static void Open()
        {
            GetWindowWithRect<WelcomeWindow>(new Rect(0, 0, 449, 530), true, "UDebugPanel - Welcome", true);
        }
        
#if UDEBUG_PANEL_DEVELOPMENT
        [MenuItem(EditorPathsConstants.DevelopmentClearPrefsItemPath)]
        public static void ClearWelcomeWindowPrefs()
        {
            EditorPrefs.SetString(WelcomeWindowShownForVersionPlayerPrefsKey, string.Empty);
        }
#endif
        
        void OnEnable()
        {
            EditorPrefs.SetString(WelcomeWindowShownForVersionPlayerPrefsKey, UDebugPanel.Version);
        }

        public static bool ShouldOpen()
        {
            bool hasKey = EditorPrefs.HasKey(WelcomeWindowShownForVersionPlayerPrefsKey);

            if (!hasKey)
            {
                return true;
            }

            string value = EditorPrefs.GetString(WelcomeWindowShownForVersionPlayerPrefsKey);

            if (value != UDebugPanel.Version)
            {
                return true;
            }

            return false;
        }
        
        void OnGUI()
        {
            DrawLogo(GetWelcomeLogo());
            
            GUILayout.Label("Welcome to UDebug Panel!", UDebugPanelEditorStylesUtils.BigTitleStyle);
            
            GUILayout.Label($"Version {UDebugPanel.Version}", UDebugPanelEditorStylesUtils.VersionNumberStyle);
            
            GUILayout.Space(10);
            GUILayout.Label("Thank you for purchasing UDebug Panel! Here are some useful links:", UDebugPanelEditorStylesUtils.WordWrappedLabelCenteredStyle);

            GUILayout.Space(10);

            if (GUILayout.Button("Examples", UDebugPanelEditorStylesUtils.BigButtonStyle))
            {
                PingExamplesFolder();
            }
            
            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL(ExternalLinksConstants.DocumentationUrl);
            }
            
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Changelog"))
                {
                    Application.OpenURL(ExternalLinksConstants.ChangelogUrl);
                }

                if (GUILayout.Button("Issues/Request"))
                {
                    Application.OpenURL(ExternalLinksConstants.IssuesRequestsUrl);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Support Form"))
                {
                    Application.OpenURL(ExternalLinksConstants.SupportFormUrl);
                }

                if (GUILayout.Button("Support Mail"))
                {
                    Application.OpenURL(ExternalLinksConstants.SupportMailUrl);
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Rate Asset!"))
            {
                Application.OpenURL(ExternalLinksConstants.ReviewUrl);
            }
        }
        
        void DrawLogo(Texture2D? logo)
        {
            if (logo == null)
            {
                return;
            }
            
            GUILayout.Space(15);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                Rect logoRect = GUILayoutUtility.GetRect(
                    logo.width,
                    logo.width,
                    logo.height,
                    logo.height,
                    GUILayout.ExpandHeight(false)
                );

                GUI.DrawTexture(logoRect, logo);

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
        
        Texture2D? GetWelcomeLogo()
        {
            if (_welcomeLogoTexture != null)
            {
                return _welcomeLogoTexture;
            }

            string? path = UDebugPanelEditorUtils.FindAssetPathByAssetName("DebugPanelLogo");

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            
            path = Path.GetFileNameWithoutExtension(path);
            
            _welcomeLogoTexture = Resources.Load<Texture2D>(path);

            return _welcomeLogoTexture;
        }
        
        void PingExamplesFolder()
        {
            string? path = UDebugPanelEditorUtils.FindAssetPathByAssetName("DebugPanel.Game");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            path = Path.GetDirectoryName(path);
            Object folderAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
            
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(folderAsset);
        }
    }
}