using System;
using TMPro;
using DebugPanel.Behaviours;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class IntSelectorDebugActionWidget : DebugActionWidget
    {
        public TextMeshProUGUI Label;
        public TMP_InputField InputField;
        public Button LeftButton;
        public Button RightButton;

        int _step;
        Action<int> _setAction;
        Func<int> _getAction;
    
        public void Init(string widgetName, int step, Action<int> setAction, Func<int> getAction)
        {
            _setAction = setAction;
            _getAction = getAction;
            _step = step;
            
            Label.text = widgetName;
            InputField.text = getAction.Invoke().ToString();
            InputField.onSubmit.AddListener(WhenValueChanged);
            
            LeftButton.onClick.AddListener(StepLeft);
            RightButton.onClick.AddListener(StepRight);
        }

        void WhenValueChanged(string newValueString)
        {
            bool isValidInt = int.TryParse(newValueString, out int newValue);

            if (!isValidInt)
            {
                return;
            }

            SetNewValue(newValue);
        }

        void StepLeft()
        {
            int currentValue = _getAction.Invoke();
            SetNewValue(currentValue - _step);
        }

        void StepRight()
        {
            int currentValue = _getAction.Invoke();
            SetNewValue(currentValue + _step);
        }

        void SetNewValue(int value)
        {
            _setAction.Invoke(value);
            value = _getAction.Invoke();
            InputField.SetTextWithoutNotify(value.ToString());
        }

        public override void Refresh()
        {
            int value = _getAction.Invoke();
            InputField.SetTextWithoutNotify(value.ToString());
        }
    }
}