#nullable enable

using System;
using System.Collections.Generic;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Raiseables;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class ButtonConsoleDebugAction : DebugAction
    {
        public Func<IReadOnlyList<string>> LogsAction { get; }
        public IListenRaiseable<string>? AddLogRaiseable { get; }
        
        public ButtonConsoleDebugAction(
            string actionName, 
            Func<IReadOnlyList<string>> logsAction,
            IListenRaiseable<string>? addLogRaiseable
            )
        {
            ActionName = actionName;
            LogsAction = logsAction;
            AddLogRaiseable = addLogRaiseable;
        }

        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            ButtonConsoleDebugActionWidget widget = (ButtonConsoleDebugActionWidget)viewInstance;
            widget.Init(
                ActionName,
                LogsAction,
                AddLogRaiseable
            );
        }
    }
}