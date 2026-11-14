using System;
using System.Threading.Tasks;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Views;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class ButtonLargeInfoAsyncDebugAction : DebugAction
    {
        public Func<Task<string>> InfoAction { get; }
        
        public ButtonLargeInfoAsyncDebugAction(string actionName, Func<Task<string>> infoAction)
        {
            ActionName = actionName;
            InfoAction = infoAction;
        }

        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            ButtonLargeInfoDebugActionWidget widget = (ButtonLargeInfoDebugActionWidget)viewInstance;
            widget.Init(ActionName, InfoAction);
        }
    }
}