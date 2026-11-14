#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DebugPanel.DebugActions.Actions;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Raiseables;
using UnityEngine;

namespace DebugPanel.Extensions
{
    public static class DebugActionsContainerExtensions
    {
        /// <summary>
        /// Adds an debug action widget in the form of a static label with the
        /// provided information text.
        /// </summary>
        /// <param name="section">The debug actions section to add the information action to.</param>
        /// <param name="info">The information text to be displayed.</param>
        /// <returns>The added information debug action.</returns>
        public static InfoDebugAction AddInfo(this IDebugActionsContainer section, string info)
        {
            InfoDebugAction debugAction = new InfoDebugAction(info);
            section.Add(debugAction);
            return debugAction;
        }
        
        /// <summary>
        /// Adds an debug action widget in the form of a label that gets updated every frame
        /// with the provided information text.
        /// </summary>
        /// <param name="section">The debug actions section to add the dynamic information action to.</param>
        /// <param name="getInfoAction">The function to retrieve dynamic information text.</param>
        /// <returns>The added dynamic information debug action.</returns>
        public static DynamicInfoDebugAction AddInfoDynamic(this IDebugActionsContainer section, Func<string> getInfoAction)
        {
            DynamicInfoDebugAction debugAction = new DynamicInfoDebugAction(getInfoAction);
            section.Add(debugAction);
            return debugAction;
        }
    
        /// <summary>
        /// Adds a debug action widget in the form of a button, with the provided name
        /// and action to be executed when the button is clicked.
        /// </summary>
        /// <param name="section">The debug actions section to add the button action to.</param>
        /// <param name="name">The name of the button.</param>
        /// <param name="action">The action to be executed when the button is clicked.</param>
        /// <param name="closePanelOnInput">Closes the debug panel once the user has performed input.</param>
        /// <returns>The added button debug action.</returns>
        public static ButtonDebugAction AddButton(this IDebugActionsContainer section, string name, Action action, bool closePanelOnInput = false)
        {
            ButtonDebugAction debugAction = new ButtonDebugAction(name, action, closePanelOnInput);
            section.Add(debugAction);
            return debugAction;
        }
    
        /// <summary>
        /// Adds a debug action widget in the form of a toggle, with the provided name
        /// and actions for setting and getting the toggle state.
        /// </summary>
        /// <param name="section">The debug actions section to add the toggle action to.</param>
        /// <param name="name">The name of the toggle.</param>
        /// <param name="setAction">The action to set the toggle state.</param>
        /// <param name="getAction">The function to get the current toggle state.</param>
        /// <param name="closePanelOnInput">Closes the debug panel once the user has performed input.</param>
        /// <returns>The added toggle debug action.</returns>
        public static ToggleDebugAction AddToggle(
            this IDebugActionsContainer section, 
            string name, 
            Action<bool> setAction, 
            Func<bool> getAction, 
            bool closePanelOnInput = false
            )
        {
            ToggleDebugAction debugAction = new ToggleDebugAction(name, setAction, getAction, closePanelOnInput);
            section.Add(debugAction);
            return debugAction;
        }
        
        /// <summary>
        /// Adds an debug action widget in the form of an int selector, with the provided name,
        /// step, and actions for setting and getting the integer value.
        /// </summary>
        /// <param name="section">The debug actions section to add the integer action to.</param>
        /// <param name="name">The name of the integer.</param>
        /// <param name="step">The step value used for increasing or decreasing the integer value.</param>
        /// <param name="setAction">The action to set the integer value.</param>
        /// <param name="getAction">The function to get the current integer value.</param>
        /// <returns>The added integer debug action.</returns>
        public static IntSelectorDebugAction AddIntSelector(
            this IDebugActionsContainer section, 
            string name, 
            int step, 
            Action<int> setAction, 
            Func<int> getAction
            )
        {
            IntSelectorDebugAction debugAction = new IntSelectorDebugAction(name, step, setAction, getAction);
            section.Add(debugAction);
            return debugAction;
        }
    
        /// <summary>
        /// Adds an debug action widget in the form of an int selector, with the provided name,
        /// step, and actions for setting and getting the integer value.
        /// Uses a default step value of 1.
        /// </summary>
        /// <param name="section">The debug actions section to add the integer action to.</param>
        /// <param name="name">The name of the integer.</param>
        /// <param name="setAction">The action to set the integer value.</param>
        /// <param name="getAction">The function to get the current integer value.</param>
        /// <returns>The added integer debug action.</returns>
        public static IntSelectorDebugAction AddIntSelector(
            this IDebugActionsContainer section, 
            string name, 
            Action<int> setAction, 
            Func<int> getAction
            )
        {
            return section.AddIntSelector(name, 1, setAction, getAction);
        }
        
        /// <summary>
        /// Adds an debug action widget in the form of a long selector, with the provided name,
        /// step, and actions for setting and getting the long value.
        /// </summary>
        /// <param name="section">The debug actions section to add the long action to.</param>
        /// <param name="name">The name of the long.</param>
        /// <param name="step">The step value used for increasing or decreasing the long value.</param>
        /// <param name="setAction">The action to set the long value.</param>
        /// <param name="getAction">The function to get the current long value.</param>
        /// <returns>The added long debug action.</returns>
        public static LongSelectorDebugAction AddLongSelector(
            this IDebugActionsContainer section, 
            string name, 
            long step, 
            Action<long> setAction, 
            Func<long> getAction
            )
        {
            LongSelectorDebugAction debugAction = new LongSelectorDebugAction(name, step, setAction, getAction);
            section.Add(debugAction);
            return debugAction;
        }
    
        /// <summary>
        /// Adds an debug action widget in the form of a long selector, with the provided name,
        /// step, and actions for setting and getting the long value.
        /// Uses a default step value of 1.
        /// </summary>
        /// <param name="section">The debug actions section to add the long action to.</param>
        /// <param name="name">The name of the integer.</param>
        /// <param name="setAction">The action to set the long value.</param>
        /// <param name="getAction">The function to get the current long value.</param>
        /// <returns>The added long debug action.</returns>
        public static LongSelectorDebugAction AddLongSelector(
            this IDebugActionsContainer section, 
            string name, 
            Action<long> setAction, 
            Func<long> getAction
            )
        {
            return section.AddLongSelector(name, 1, setAction, getAction);
        }
        
        /// <summary>
        /// Adds a debug action widget in the form of a float selector, with the provided name,
        /// step, and actions for setting and getting the float value.
        /// </summary>
        /// <param name="section">The debug actions section to add the float action to.</param>
        /// <param name="name">The name of the float.</param>
        /// <param name="step">The step value for increasing or decreasing the float value.</param>
        /// <param name="setAction">The action to set the float value.</param>
        /// <param name="getAction">The function to get the current float value.</param>
        /// <returns>The added float debug action.</returns>
        public static FloatSelectorDebugAction AddFloatSelector(
            this IDebugActionsContainer section,
            string name, 
            float step, 
            Action<float> setAction, 
            Func<float> getAction
            )
        {
            FloatSelectorDebugAction debugAction = new FloatSelectorDebugAction(name, step, setAction, getAction);
            section.Add(debugAction);
            return debugAction;
        }
        
        /// <summary>
        /// Adds a debug action widget in the form of a float selector, with the provided name,
        /// step, and actions for setting and getting the float value.
        /// Uses a default step value of 0.1f.
        /// </summary>
        /// <param name="section">The debug actions section to add the float action to.</param>
        /// <param name="name">The name of the float.</param>
        /// <param name="setAction">The action to set the float value.</param>
        /// <param name="getAction">The function to get the current float value.</param>
        /// <returns>The added float debug action.</returns>
        public static FloatSelectorDebugAction AddFloatSelector(
            this IDebugActionsContainer section, 
            string name, 
            Action<float> setAction, 
            Func<float> getAction
            )
        {
            return section.AddFloatSelector(name, 1f, setAction, getAction);
        }
        
        [Obsolete("Deprecated, please use AddButtonListInput instead")]
        public static ButtonListInputDebugAction AddButtonListSelector(
            this IDebugActionsContainer section, 
            string name, 
            Func<IReadOnlyList<string>> getElementsFunc, 
            Action<int> whenSelectedAction,
            bool closePanelOnInput = false
        )
        {
            return section.AddButtonListInput(name, getElementsFunc, whenSelectedAction, closePanelOnInput);
        }
        
        /// <summary>
        /// Adds a debug action widget in the form of a button, that when pressed, opens a
        /// list of elements to select.
        /// </summary>
        /// <param name="section">The debug actions container section where the button
        /// list selector will be added.</param>
        /// <param name="name">The name of the button list selector.</param>
        /// <param name="getElementsFunc">A function that asks for a list of string
        /// elements for selection.</param>
        /// <param name="whenSelectedAction">An action that is invoked when an element
        /// is selected, receiving the selected index.</param>
        /// <param name="closePanelOnInput">If true, the debug panel will close when an
        /// element is selected.</param>
        /// <returns>Returns the created ButtonListSelectorDebugAction instance.</returns>
        public static ButtonListInputDebugAction AddButtonListInput(
            this IDebugActionsContainer section, 
            string name, 
            Func<IReadOnlyList<string>> getElementsFunc, 
            Action<int> whenSelectedAction,
            bool closePanelOnInput = false
            )
        {
            ButtonListInputDebugAction debugAction = new ButtonListInputDebugAction(name, getElementsFunc, whenSelectedAction, closePanelOnInput);
            section.Add(debugAction);
            return debugAction;
        }
        
        [Obsolete("Deprecated, please use AddButtonListInputAsync instead")]
        public static ButtonListInputDebugAction AddButtonListSelectorAsync(
            this IDebugActionsContainer section, 
            string name, 
            Func<Task<IReadOnlyList<string>>> getElementsFunc, 
            Action<int> whenSelectedAction,
            bool closePanelOnInput = false
        )
        {
            return section.AddButtonListInputAsync(name, getElementsFunc, whenSelectedAction, closePanelOnInput);
        }
        
        /// <summary>
        /// Adds a debug action widget in the form of a button, that when presse, opens a
        /// list of elements to select.
        /// </summary>
        /// <param name="section">The debug actions container section where the button
        /// list selector will be added.</param>
        /// <param name="name">The name of the button list selector.</param>
        /// <param name="getElementsFunc">A function that asynchronously asks for a 
        /// list of string elements for selection.</param>
        /// <param name="whenSelectedAction">An action that is invoked when an element is selected,
        /// receiving the selected index.</param>
        /// <param name="closePanelOnInput">If true, the debug panel will close when an element is
        /// selected.</param>
        /// <returns>Returns the created ButtonListSelectorDebugAction instance.</returns>
        public static ButtonListInputDebugAction AddButtonListInputAsync(
            this IDebugActionsContainer section, 
            string name, 
            Func<Task<IReadOnlyList<string>>> getElementsFunc, 
            Action<int> whenSelectedAction,
            bool closePanelOnInput = false
        )
        {
            ButtonListInputDebugAction debugAction = new ButtonListInputDebugAction(name, getElementsFunc, whenSelectedAction, closePanelOnInput);
            section.Add(debugAction);
            return debugAction;
        }
        
                
        [Obsolete("Deprecated, please use AddButtonListSelector instead")]
        public static ButtonListSelectorDebugAction AddButtonElementListSelector(
            this IDebugActionsContainer section, 
            string name, 
            IReadOnlyList<string> elements, 
            Action<int> setAction, 
            Func<int> getAction,
            bool refreshEveryFrame = false
        )
        {
            return section.AddButtonListSelector(name, elements, setAction, getAction, refreshEveryFrame);
        }
        
        /// <summary>
        /// Adds a debug action widget in the form of a button, that when pressed,
        /// opens a list of elements to select. When an element is selected, tha value
        /// stays stored as a variable.
        /// </summary>
        /// <param name="section">The debug actions container section where the
        /// button element list selector will be added.</param>
        /// <param name="name">The name of the button element list selector.</param>
        /// <param name="elements">A read-only list of string elements available for selection.</param>
        /// <param name="setAction">An action that sets the selected index.</param>
        /// <param name="getAction">A function that gets the currently selected index.</param>
        /// <param name="refreshEveryFrame">If true, the list will be refreshed every frame.</param>
        /// <returns>Returns the created ButtonElementListSelectorDebugAction instance.</returns>
        public static ButtonListSelectorDebugAction AddButtonListSelector(
            this IDebugActionsContainer section, 
            string name, 
            IReadOnlyList<string> elements, 
            Action<int> setAction, 
            Func<int> getAction,
            bool refreshEveryFrame = false
        )
        {
            ButtonListSelectorDebugAction debugAction = new ButtonListSelectorDebugAction(name, elements, setAction, getAction, refreshEveryFrame);
            section.Add(debugAction);
            return debugAction;
        }
        
        /// <summary>
        /// Adds an debug action widget in the form of a button that, when pressed, opens an
        /// enum value selection popup. When an element is selected, tha value
        /// stays stored as a variable.
        /// </summary>
        /// <param name="section">The debug actions section to add the enum action to.</param>
        /// <param name="name">The name of the enum action.</param>
        /// <param name="setAction">The action to set the enum value.</param>
        /// <param name="getAction">The function to get the current enum value.</param>
        /// <param name="refreshEveryFrame">If true, the list will be refreshed every frame.</param>
        /// <returns>Returns the created ButtonElementListSelectorDebugAction instance.</returns>
        public static IDebugAction AddButtonEnumSelector<T>(
            this IDebugActionsContainer section, 
            string name, 
            Action<T> setAction, 
            Func<T> getAction,
            bool refreshEveryFrame = false
            )
            where T : Enum
        {
            Type enumType = typeof(T);
            
            Array enumValues = Enum.GetValues(enumType);
            
            List<T> values = new();
            List<string> elements = new();
            
            foreach (T value in enumValues)
            {
                values.Add(value);
                elements.Add(value.ToString());
            }
            
            void WhenSet(int index)
            {
                T element = values[index];
                setAction.Invoke(element);
            }

            int WhenGet()
            {
                T element = getAction.Invoke();
                return Convert.ToInt32(element);
            }
            
            return section.AddButtonListSelector(name, elements, WhenSet, WhenGet, refreshEveryFrame);
        }
        
        /// <summary>
        /// Adds an debug action widget in the form of a button that, when pressed, opens an
        /// enum value selection popup. When an element is selected, tha value
        /// stays stored as a variable.
        /// </summary>
        /// <param name="section">The debug actions section to add the enum action to.</param>
        /// <param name="name">The name of the enum action.</param>
        /// <param name="enumType">The enum type to select.</param>
        /// <param name="setAction">The action to set the enum value.</param>
        /// <param name="getAction">The function to get the current enum value.</param>
        /// <param name="refreshEveryFrame">If true, the list will be refreshed every frame.</param>
        /// <returns>Returns the created ButtonElementListSelectorDebugAction instance.</returns>
        public static IDebugAction AddButtonEnumSelector(
            this IDebugActionsContainer section, 
            string name, 
            Type enumType,
            Action<Enum> setAction, 
            Func<Enum> getAction,
            bool refreshEveryFrame = false
        )
        {
            if (!enumType.IsEnum)
            {
                return NothingDebugAction.Instance;
            }
            
            Array enumValues = Enum.GetValues(enumType);

            List<Enum> values = new();
            List<string> elements = new();
            
            foreach (Enum value in enumValues)
            {
                values.Add(value);
                elements.Add(value.ToString());
            }

            void WhenSet(int index)
            {
                Enum element = values[index];
                setAction.Invoke(element);
            }

            int WhenGet()
            {
                Enum element = getAction.Invoke();
                return Convert.ToInt32(element);
            }
            
            return section.AddButtonListSelector(name, elements, WhenSet, WhenGet, refreshEveryFrame);
        }
        
        /// <summary>
        /// Adds a debug action widget in the form of a button that, when pressed, opens
        /// a popup with the provided info.
        /// </summary>
        /// <param name="section">The debug actions container section where the button will be added.</param>
        /// <param name="name">The name of the button.</param>
        /// <param name="getInfo">A function that returns the string information to be
        /// displayed when the button is pressed.</param>
        /// <param name="refreshEveryFrame">If true, the info will be refreshed every frame.</param>
        /// <returns>Returns the created ButtonLargeInfoDebugAction instance.</returns>
        public static ButtonLargeInfoDebugAction AddButtonLargeInfo(
            this IDebugActionsContainer section, 
            string name, 
            Func<string> getInfo,
            bool refreshEveryFrame = false
        )
        {
            ButtonLargeInfoDebugAction debugAction = new ButtonLargeInfoDebugAction(name, getInfo, refreshEveryFrame);
            section.Add(debugAction);
            return debugAction;
        }
        
        /// <summary>
        /// Adds a debug action widget in the form of a button that, when pressed, opens
        /// a popup with the provided info.
        /// </summary>
        /// <param name="section">The debug actions container section where the button will be added.</param>
        /// <param name="name">The name of the button.</param>
        /// <param name="getInfo">A function that returns the string information to be
        /// displayed when the button is pressed.</param>
        /// <returns>Returns the created ButtonLargeInfoDebugAction instance.</returns>
        public static ButtonLargeInfoAsyncDebugAction AddButtonLargeInfoAsync(
            this IDebugActionsContainer section, 
            string name, 
            Func<Task<string>> getInfo
        )
        {
            ButtonLargeInfoAsyncDebugAction debugAction = new ButtonLargeInfoAsyncDebugAction(name, getInfo);
            section.Add(debugAction);
            return debugAction;
        }
        
                
        /// <summary>
        /// Adds a debug action widget in the form of a button that, when pressed, opens a popup where you can
        /// set a string value.
        /// </summary>
        /// <param name="section">The debug actions container to which the button will be added.</param>
        /// <param name="name">The name of the text input button.</param>
        /// <param name="initialTextAction">A function that provides the initial text value.</param>
        /// <param name="inputAction">An action to handle the input text.</param>
        /// <returns>A <see cref="ButtonStringInputDebugAction"/> instance representing the added text input button.</returns>
        public static ButtonStringInputDebugAction AddButtonStringInput(
            this IDebugActionsContainer section, 
            string name, 
            Func<string> initialTextAction,
            Action<string> inputAction
        )
        {
            ButtonStringInputDebugAction debugAction = new ButtonStringInputDebugAction(name, initialTextAction, inputAction);
            section.Add(debugAction);
            return debugAction;
        }
        
        /// <summary>
        /// Adds a debug action widget represented as a button that displays the current string value.
        /// When pressed, it opens a popup that allows the user to input or select a new string.
        /// The selected string is then reflected in the button label.
        /// </summary>
        /// <param name="section">The debug actions container to which the button will be added.</param>
        /// <param name="name">The label of the button.</param>
        /// <param name="setAction">An action that sets the selected string value.</param>
        /// <param name="getAction">A function that retrieves the current string value to display.</param>
        /// <returns>
        /// A <see cref="ButtonStringSelectorDebugAction"/> representing the interactive string input button.
        /// </returns>
        public static ButtonStringSelectorDebugAction AddButtonStringSelector(
            this IDebugActionsContainer section, 
            string name,
            Action<string> setAction,
            Func<string> getAction
        )
        {
            ButtonStringSelectorDebugAction debugAction = new ButtonStringSelectorDebugAction(name, setAction, getAction);
            section.Add(debugAction);
            return debugAction;
        }
        
        /// <summary>
        /// Adds a debug action widget in the form of a button that, when pressed,
        /// opens a popup with a log console.
        /// </summary>
        /// <param name="section">The debug actions container to which the button will be added.</param>
        /// <param name="name">The name of the button.</param>
        /// <param name="getLogs">A function that returns a read-only list of log messages.</param>
        /// <param name="addLog">An optional log addition handler.</param>
        /// <returns>A <see cref="ButtonConsoleDebugAction"/> instance representing the added button.</returns>
        public static ButtonConsoleDebugAction AddButtonConsole(
            this IDebugActionsContainer section, 
            string name, 
            Func<IReadOnlyList<string>> getLogs,
            IListenRaiseable<string>? addLog
        )
        {
            ButtonConsoleDebugAction debugAction = new ButtonConsoleDebugAction(name, getLogs, addLog);
            section.Add(debugAction);
            return debugAction;
        }

        /// <summary>
        /// Adds a debug action widget in the form of a button that, when pressed, opens a
        /// popup with the specified debug actions.
        /// </summary>
        /// <param name="section">The debug actions container to which the button will be added.</param>
        /// <param name="name">The name of the button actions container.</param>
        /// <returns>A <see cref="ButtonActionsContainerDebugAction"/> instance representing
        /// the added button actions container.</returns>
        public static ButtonActionsContainerDebugAction AddButtonActionsContainer(
            this IDebugActionsContainer section,
            string name
        )
        {
            ButtonActionsContainerDebugAction debugAction = new ButtonActionsContainerDebugAction(name);
            section.Add(debugAction);
            return debugAction;
        }

        /// <summary>
        /// Adds a debug action widget in the form of a button that, when pressed, opens a
        /// popup with the specified debug actions.
        /// </summary>
        /// <param name="section">The debug actions container to which the button will be added.</param>
        /// <param name="name">The name of the button actions container.</param>
        /// <param name="buildAction">An action that will be executed when the container is instantiated.</param>
        /// <returns>A <see cref="ButtonActionsContainerDebugAction"/> instance representing
        /// the added button actions container.</returns>
        public static ButtonActionsContainerDebugAction AddButtonActionsContainer(
            this IDebugActionsContainer section,
            string name,
            Action<IDebugActionsContainer> buildAction
        )
        {
            ButtonActionsContainerDebugAction debugAction = new ButtonActionsContainerDebugAction(name);
            
            debugAction.OnInstantiate += buildAction;
            debugAction.ClearOnInstantiate = true;
            
            section.Add(debugAction);
            return debugAction;
        }
        
        public static ActionsGroupDebugAction AddActionsGroup(
            this IDebugActionsContainer section,
            string name
        )
        {
            ActionsGroupDebugAction debugAction = new ActionsGroupDebugAction(name);
            section.Add(debugAction);
            return debugAction;
        }
    }
}