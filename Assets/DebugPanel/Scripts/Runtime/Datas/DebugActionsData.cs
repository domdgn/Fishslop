using System.Collections.Generic;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Containers;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Views;

namespace DebugPanel.Datas
{
    public sealed class DebugActionsData
    {
        public readonly List<DebugActionsSection> Sections = new();
        public readonly Dictionary<DebugActionsSection, DebugPanelSectionView> SectionsViews = new();
        public readonly Dictionary<IDebugAction, DebugActionWidget> Widgets = new();
    }
}