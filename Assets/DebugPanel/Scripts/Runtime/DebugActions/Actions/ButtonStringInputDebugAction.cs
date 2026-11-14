using System;
using DebugPanel.DebugActions.Widgets;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class ButtonStringInputDebugAction : DebugAction
    {
        public Func<string> InitialTextAction { get; }
        public Action<string> InputAction { get; }
        
        public ButtonStringInputDebugAction(
            string actionName, 
            Func<string> initialTextAction,
            Action<string> inputAction
        )
        {
            ActionName = actionName;
            InitialTextAction = initialTextAction;
            InputAction = inputAction;
        }

        public override void InitWidget(
            DebugActionWidget viewInstance
        )
        {
            ButtonStringInputDebugActionWidget widget = (ButtonStringInputDebugActionWidget)viewInstance;
            widget.Init(ActionName, InitialTextAction, InputAction);
        }
    }
}