using System;
using DebugPanel.DebugActions.Containers;
using UnityEngine;

namespace DebugPanel.Examples
{
    public static class ExampleCustomDebugActionsContainerExtensions
    {
        public static ExampleCustomDebugAction AddDynamicVector3(this IDebugActionsContainer section, Func<Vector3> vector3)
        {
            ExampleCustomDebugAction debugAction = new ExampleCustomDebugAction(vector3);
            section.Add(debugAction);
            return debugAction;
        }
    }
}