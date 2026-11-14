using System;
using DebugPanel.DebugActions.Widgets;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class ButtonLargeInfoDebugAction : DebugAction
    {
        public Func<string> InfoAction { get; }
        public bool RefreshEveryFrame { get; }
        
        public ButtonLargeInfoDebugAction(string actionName, Func<string> infoAction, bool refreshEveryFrame)
        {
            ActionName = actionName;
            InfoAction = infoAction;
            RefreshEveryFrame = refreshEveryFrame;
        }

        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            ButtonLargeInfoDebugActionWidget widget = (ButtonLargeInfoDebugActionWidget)viewInstance;
            widget.Init(ActionName, InfoAction, RefreshEveryFrame);
        }
    }
}