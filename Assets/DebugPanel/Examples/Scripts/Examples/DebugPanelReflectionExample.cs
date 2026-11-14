using System.ComponentModel;
using DebugPanel.Attributes;
using DebugPanel.Core;
using UnityEngine;

namespace DebugPanel.Examples
{
    /// <summary>
    /// Example demonstrating how to use an Options class to automatically
    /// generate debug actions from it, by using reflection.
    /// </summary>
    public sealed class DebugPanelReflectionExample : MonoBehaviour
    {
        enum EnumProperty
        {
            Value1,
            Value2,
            Value3,
        }
        
        sealed class Options
        {
            public string StringProperty { get; set; } = "Empty";
            
            [NumberStep(5)]
            public int IntProperty { get; set; }
            
            [NumberStep(2.5f)] 
            public float FloatProperty { get; set; }
            
            [NumberRange(-10, 10)] 
            public long LongProperty { get; set; }
            
            [DisplayName("Bool Property Custom Display Name")]
            public bool BoolProperty { get; set; }
            public EnumProperty EnumProperty { get; set; }
            
            public string StringValue { get; } = "Text";
            public int IntValue => IntProperty;
            public float FloatValue => FloatProperty;
            public long LongValue => LongProperty;
            public bool BoolValue => BoolProperty;
            
            [DontShowAsDebugAction]
            public EnumProperty EnumValue => EnumProperty;
            
            [ShowAsDebugAction]
            EnumProperty PrivateEnumValue { get; set; }
            
            public void Method()
            {
                Debug.Log("Calling Method");
            }
            
            [ShowAsDebugAction]
            void PrivateMethod()
            {
                Debug.Log("Calling Private Method");
            }
            
            [Category("Another Section")]
            public int IntProperty2 { get; set; }
            
            [Category("Another Section")]
            public int FloatProperty2 { get; set; }
        }

        [DebugActionsObject]
        static class OptionsAutomatic
        {
            public static int IntProperty { get; set; }
            
            [NumberStep(2.5f)] 
            public static float FloatProperty { get; set; }
            
            [NumberRange(-10, 10)] 
            public static long LongProperty { get; set; }
        }

        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.Show();
            
            // Manually ads an object to create debug actions.
            UDebugPanel.AddOptionsObject(new Options());
            
            // Gathers all static objects on the project that have the
            // DebugActionsObject attribute, and creates debug actions.
            UDebugPanel.AddGlobalOptionsObjects();
        }
    }
}