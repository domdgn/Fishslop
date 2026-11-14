using System;
using System.Collections.Generic;
using DebugPanel.DebugActions.Widgets;

namespace DebugPanel.Datas
{
    public sealed class DebugActionsPrefabsData
    {
        public readonly Dictionary<Type, DebugActionWidget> DebugActionTypeByDebugActionWidgetPrefabs = new();
    }
}