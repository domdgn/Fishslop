using System;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Widgets;
using UnityEngine;

namespace DebugPanel.Examples
{
    public sealed class ExampleCustomDebugAction : DebugAction
    {
        public Func<Vector3> GetVector3;

        public ExampleCustomDebugAction(Func<Vector3> getVector3)
        {
            GetVector3 = getVector3;
        }

        public override void InitWidget(DebugActionWidget viewInstance)
        {
            ExampleCustomDebugActionWidget widget = (ExampleCustomDebugActionWidget)viewInstance;
            widget.Init(GetVector3);
        }
    }
}