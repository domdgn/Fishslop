using System;
using DebugPanel.DebugActions.Widgets;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class ButtonStringSelectorDebugAction : DebugAction
    {
        public Action<string> SetAction { get; }
        public Func<string> GetAction { get; }
        
        public ButtonStringSelectorDebugAction(
            string actionName, 
            Action<string> setAction,
            Func<string> getAction
        )
        {
            ActionName = actionName;
            SetAction = setAction;
            GetAction = getAction;
        }

        public override void InitWidget(
            DebugActionWidget viewInstance
        )
        {
            ButtonStringSelectorDebugActionWidget widget = (ButtonStringSelectorDebugActionWidget)viewInstance;
            widget.Init(ActionName, SetAction, GetAction);
        }
    }
}