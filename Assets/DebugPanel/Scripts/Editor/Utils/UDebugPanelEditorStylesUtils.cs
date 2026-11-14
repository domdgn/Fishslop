#nullable enable

using UnityEditor;
using UnityEngine;

namespace DebugPanel.Utils
{
    public static class UDebugPanelEditorStylesUtils
    {
        static GUIStyle? _bigTitleStyle;
        static GUIStyle? _wordWrappedLabelCenteredStyle;
        static GUIStyle? _bigButtonStyle;
        static GUIStyle? _versionNumberStyle;

        public static GUIStyle BigTitleStyle =>
            _bigTitleStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

        public static GUIStyle WordWrappedLabelCenteredStyle =>
            _wordWrappedLabelCenteredStyle ??= new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };

        public static GUIStyle BigButtonStyle =>
            _bigButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

        public static GUIStyle VersionNumberStyle =>
            _versionNumberStyle ??= new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
            };
    }
}
        