using System;
using DebugPanel.DebugActions.Widgets;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class ButtonDebugAction : DebugAction
    {
        public Action Action { get; }
        public bool ClosePanelOnInput { get; }

        public ButtonDebugAction(string actionName, Action action, bool closePanelOnInput)
        {
            ActionName = actionName;
            Action = action;
            ClosePanelOnInput = closePanelOnInput;
        }

        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            ButtonDebugActionWidget widget = (ButtonDebugActionWidget)viewInstance;
            widget.Init(ActionName, Action, ClosePanelOnInput);
        }
    }
}