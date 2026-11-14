#nullable enable
using System;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Views;

namespace DebugPanel.Datas
{
    public sealed class DebugPanelData
    {
        public DebugPanelView? DebugPanelView;
        public float ScaleFactor = 1f;
        public int SortOrder = 32767;
        
        public bool IsVisible;
        public bool IsLoadingSections;
        public bool IsLoadingWidgets;

        public string ShortcutName = string.Empty;
        public Action? ShortcutAction;
        
        public float SectionsScrollPosition;
        public float WidgetsScrollPosition;
        public string SearchFilter = string.Empty;

        public DebugActionsSection? SelectedSection;
        public bool IsUsingLastSectionAdded = true;
        public bool ShouldTryToFindSelectedSection;
    }
}