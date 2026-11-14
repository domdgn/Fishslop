using DebugPanel.DebugActions.Widgets;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class InfoDebugAction : DebugAction
    {
        public InfoDebugAction(string info)
        {
            ActionName = info;
        }
    
        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            InfoDebugActionWidget widget = (InfoDebugActionWidget)viewInstance;
            widget.Init(ActionName);
        }
    }
}