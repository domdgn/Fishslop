using System;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Views;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class LongSelectorDebugAction : DebugAction
    {
        public long Step { get; }
        public Action<long> SetAction { get; }
        public Func<long> GetAction { get; }
    
        public LongSelectorDebugAction(string actionName, long step, Action<long> setAction, Func<long> getAction)
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
            LongSelectorDebugActionWidget widget = (LongSelectorDebugActionWidget)viewInstance;
            widget.Init(ActionName, Step, SetAction, GetAction);
        }
    }
}