using System;
using System.Collections.Generic;
using DebugPanel.DebugActions.Widgets;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class ButtonListSelectorDebugAction : DebugAction
    {
        public IReadOnlyList<string> Elements { get; }
        public Action<int> SetAction { get; }
        public Func<int> GetAction { get; }
        public bool RefreshEveryFrame { get; }
        
        public ButtonListSelectorDebugAction(
            string actionName, 
            IReadOnlyList<string> elements, 
            Action<int> setAction, 
            Func<int> getAction,
            bool refreshEveryFrame
            )
        {
            ActionName = actionName;
            Elements = elements;
            SetAction = setAction;
            GetAction = getAction;
            RefreshEveryFrame = refreshEveryFrame;
        }

        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            ButtonListSelectorDebugActionWidget widget = (ButtonListSelectorDebugActionWidget)viewInstance;
            widget.Init(ActionName, Elements, SetAction, GetAction, RefreshEveryFrame);
        }
    }
}