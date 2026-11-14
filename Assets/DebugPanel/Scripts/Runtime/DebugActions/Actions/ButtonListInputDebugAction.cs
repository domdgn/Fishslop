#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DebugPanel.DebugActions.Widgets;
using UnityEngine;

namespace DebugPanel.DebugActions.Actions
{
    public sealed class ButtonListInputDebugAction : DebugAction
    {
        public Func<IReadOnlyList<string>>? GetElementsFunc { get; }
        public Func<Task<IReadOnlyList<string>>>? GetElementsFuncAsync { get; }
        public Action<int> ElementSelectedAction { get; }
        public bool ClosePanelOnInput { get; }
        
        public ButtonListInputDebugAction(
            string actionName, 
            Func<IReadOnlyList<string>> getElementsFunc, 
            Action<int> elementSelectedAction,
            bool closePanelOnInput
            )
        {
            ActionName = actionName;
            GetElementsFunc = getElementsFunc;
            ElementSelectedAction = elementSelectedAction;
            ClosePanelOnInput = closePanelOnInput;
        }
        
        public ButtonListInputDebugAction(
            string actionName, 
            Func<Task<IReadOnlyList<string>>> getElementsFunc, 
            Action<int> elementSelectedAction,
            bool closePanelOnInput
        )
        {
            ActionName = actionName;
            GetElementsFuncAsync = getElementsFunc;
            ElementSelectedAction = elementSelectedAction;
            ClosePanelOnInput = closePanelOnInput;
        }

        public override void InitWidget(
            DebugActionWidget viewInstance
            )
        {
            ButtonListInputDebugActionWidget widget = (ButtonListInputDebugActionWidget)viewInstance;

            if (GetElementsFunc != null)
            {
                widget.Init(ActionName, GetElementsFunc, ElementSelectedAction, ClosePanelOnInput);
                return;
            }
            
            if(GetElementsFuncAsync != null)
            {
                widget.Init(ActionName, GetElementsFuncAsync, ElementSelectedAction, ClosePanelOnInput);
            }
        }
    }
}