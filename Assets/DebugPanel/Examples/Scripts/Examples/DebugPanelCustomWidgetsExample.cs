using DebugPanel.Core;
using DebugPanel.DebugActions.Containers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DebugPanel.Examples
{
    /// <summary>
    /// Example demonstrating how to setup a custom widgets that
    /// work seamlessly with the debug panel.
    /// </summary>
    public sealed class DebugPanelCustomWidgetsExample : MonoBehaviour
    {
        public ExampleCustomDebugActionWidget WidgetPrefab;

        float _timerTime;
        
        Vector3 _vector3;
        
        void Start()
        {
            UDebugPanel.SetupToggleInput();
            UDebugPanel.SetControllerSupport(true);
            UDebugPanel.Show();
            
            UDebugPanel.RegisterWidgetPrefab<ExampleCustomDebugAction>(WidgetPrefab);

            IDebugActionsSection section = UDebugPanel.AddSection("Custom Widgets");
            section.AddDynamicVector3(() => _vector3);
        }

        void Update()
        {
            _timerTime += Time.deltaTime;

            // Update value every second
            if (_timerTime > 1)
            {
                _timerTime = 0;
                _vector3 = new Vector3(Random.Range(-1000, 1000), Random.Range(-1000, 1000), Random.Range(-1000, 1000));
            }
        }
    }
}