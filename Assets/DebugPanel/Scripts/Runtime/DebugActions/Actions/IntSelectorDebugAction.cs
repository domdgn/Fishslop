using System;
using DebugPanel.DebugActions.Widgets;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class IntSelectorDebugAction : DebugAction
    {
        public int Step { get; }
        public Action<int> SetAction { get; }
        public Func<int> GetAction { get; }
    
        public IntSelectorDebugAction(string actionName, int step, Action<int> setAction, Func<int> getAction)
        {
            ActionName = actionName;
            Step = step;
            SetAction = setAction;
            GetAction = getAction;
        }
    
        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            IntSelectorDebugActionWidget widget = (IntSelectorDebugActionWidget)viewInstance;
            widget.Init(ActionName, Step, SetAction, GetAction);
        }
    }
}