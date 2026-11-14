using System;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Views;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class ToggleDebugAction : DebugAction
    {
        public Action<bool> SetAction { get; }
        public Func<bool> GetAction { get; }
        public bool ClosePanelOnInput { get; }
    
        public ToggleDebugAction(string actionName, Action<bool> setAction, Func<bool> getAction, bool closePanelOnInput)
        {
            ActionName = actionName;
            SetAction = setAction;
            GetAction = getAction;
            ClosePanelOnInput = closePanelOnInput;
        }
    
        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            ToggleDebugActionWidget widget = (ToggleDebugActionWidget)viewInstance;
            widget.Init(ActionName, SetAction, GetAction, ClosePanelOnInput);
        }
    }
}