using System;
using DebugPanel.DebugActions.Widgets;
using TMPro;
using UnityEngine;

namespace DebugPanel.Examples
{
    public sealed class ExampleCustomDebugActionWidget : DebugActionWidget
    {
        public TextMeshProUGUI LabelX;
        public TextMeshProUGUI LabelY;
        public TextMeshProUGUI LabelZ;

        Func<Vector3> _vector3;
        
        public void Init(Func<Vector3> vector3)
        {
            _vector3 = vector3;
            RefreshValue();
        }

        void Update()
        {
            RefreshValue();
        }

        void RefreshValue()
        {
            Vector3 vector = _vector3.Invoke();
            
            LabelX.text = $"X: {vector.x}";
            LabelY.text = $"Y: {vector.y}";
            LabelZ.text = $"Z: {vector.z}";
        }
    }
}