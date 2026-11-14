using System;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Views;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class FloatSelectorDebugAction : DebugAction
    {
        public float Step { get; }
        public Action<float> SetAction { get; }
        public Func<float> GetAction { get; }
    
        public FloatSelectorDebugAction(string actionName, float step, Action<float> setAction, Func<float> getAction)
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
            FloatSelectorDebugActionWidget widget = (FloatSelectorDebugActionWidget)viewInstance;
            widget.Init(ActionName, Step, SetAction, GetAction);
        }
    }
}