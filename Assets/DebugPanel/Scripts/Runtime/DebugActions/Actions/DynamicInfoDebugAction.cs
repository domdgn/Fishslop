using System;
using DebugPanel.DebugActions.Widgets;
using DebugPanel.Views;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class DynamicInfoDebugAction : DebugAction
    {
        private readonly Func<string> _getInfoAction;

        public override string ActionName => _getInfoAction.Invoke();
    
        public DynamicInfoDebugAction(Func<string> getInfoAction)
        {
            _getInfoAction = getInfoAction;
        }

        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            DynamicInfoDebugActionWidget widget = (DynamicInfoDebugActionWidget)viewInstance;
            widget.Init(_getInfoAction);
        }
    }
}