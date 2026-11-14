using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DebugPanel.Behaviours
{
    public sealed class InputFieldLayoutElement : MonoBehaviour, ILayoutElement
    {
        public TMP_InputField InputField;
        public float HorizontalOffset;
        public float VerticalOffset;
        public float MinWidth;
        public float MinHeight;
        
        public float minWidth => MinWidth; // Not used
        public float minHeight => MinHeight; // Not used
        public float preferredWidth => InputField != null ? InputField.preferredWidth + HorizontalOffset : 0;
        public float preferredHeight => InputField != null ? InputField.preferredHeight + VerticalOffset : 0;
        public float flexibleWidth => -1; // Not flexible
        public float flexibleHeight => -1; // Not flexible
        public int layoutPriority => 1;
        
        void Awake()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        }

        void OnDestroy()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

        public void CalculateLayoutInputHorizontal()
        {
            if (InputField != null)
            {
                InputField.CalculateLayoutInputHorizontal();   
            }
        }

        public void CalculateLayoutInputVertical()
        {
            if (InputField != null)
            {
                InputField.CalculateLayoutInputHorizontal();   
            }
        }
        
        void OnTextChanged(Object obj)
        {
            if (obj.Equals(InputField.textComponent) == false)
            {
                return;
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }
    }
}