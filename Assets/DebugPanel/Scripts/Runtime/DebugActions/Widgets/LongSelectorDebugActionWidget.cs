#nullable enable

using System;
using TMPro;
using UnityEngine.UI;

namespace DebugPanel.DebugActions.Widgets
{
    public sealed class LongSelectorDebugActionWidget : DebugActionWidget
    {
        public TextMeshProUGUI? Label;
        public TMP_InputField? InputField;
        public Button? LeftButton;
        public Button? RightButton;

        long _step;
        Action<long>? _setAction;
        Func<long>? _getAction;
    
        public void Init(string widgetName, long step, Action<long> setAction, Func<long> getAction)
        {
            _setAction = setAction;
            _getAction = getAction;
            _step = step;
            
            Label!.text = widgetName;
            InputField!.text = getAction.Invoke().ToString();
            InputField.onSubmit.AddListener(WhenValueChanged);
            
            LeftButton!.onClick.AddListener(StepLeft);
            RightButton!.onClick.AddListener(StepRight);
        }

        void WhenValueChanged(string newValueString)
        {
            bool isValidInt = long.TryParse(newValueString, out long newValue);

            if (!isValidInt)
            {
                return;
            }

            SetNewValue(newValue);
        }

        void StepLeft()
        {
            long currentValue = _getAction!.Invoke();
            SetNewValue(currentValue - _step);
        }

        void StepRight()
        {
            long currentValue = _getAction!.Invoke();
            SetNewValue(currentValue + _step);
        }

        void SetNewValue(long value)
        {
            _setAction!.Invoke(value);
            value = _getAction!.Invoke();
            InputField!.SetTextWithoutNotify(value.ToString());
        }
        
        public override void Refresh()
        {
            long value = _getAction!.Invoke();
            InputField!.SetTextWithoutNotify(value.ToString());
        }
    }
}