#nullable enable

using System;
using System.Globalization;
using TMPro;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class FloatSelectorDebugActionWidget : DebugActionWidget
    {
        public TextMeshProUGUI? Label;
        public TMP_InputField? InputField;
        public Button? LeftButton;
        public Button? RightButton;

        float _step;
        Action<float>? _setAction;
        Func<float>? _getAction;

        void Awake()
        {
            LeftButton!.onClick.AddListener(StepLeft);
            RightButton!.onClick.AddListener(StepRight);
            InputField!.onSubmit.AddListener(WhenValueChanged);
        }

        public void Init(string widgetName, float step, Action<float> setAction, Func<float> getAction)
        {
            _setAction = setAction;
            _getAction = getAction;
            _step = step;
            
            Label!.text = widgetName;
            InputField!.text = getAction.Invoke().ToString(CultureInfo.InvariantCulture);
        }

        void WhenValueChanged(string newValueString)
        {
            bool isValidFloat = float.TryParse(newValueString, out float newValue);

            if (!isValidFloat)
            {
                return;
            }

            SetNewValue(newValue);
        }

        void StepLeft()
        {
            float currentValue = _getAction!.Invoke();
            SetNewValue(currentValue - _step);
        }

        void StepRight()
        {
            float currentValue = _getAction!.Invoke();
            SetNewValue(currentValue + _step);
        }

        void SetNewValue(float value)
        {
            _setAction?.Invoke(value);
            value = _getAction!.Invoke();
            InputField!.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }
        
        public override void Refresh()
        {
            float value = _getAction!.Invoke();
            InputField!.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}