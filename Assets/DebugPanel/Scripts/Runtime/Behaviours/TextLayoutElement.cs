using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DebugPanel.Behaviours
{
    public sealed class TextLayoutElement : MonoBehaviour, ILayoutElement
    {
        public TextMeshProUGUI Text;
        public float HorizontalOffset;
        public float VerticalOffset;
        public float MinWidth;
        public float MinHeight;
        
        public float minWidth => MinWidth; // Not used
        public float minHeight => MinHeight; // Not used
        public float preferredWidth => Text != null ? Text.preferredWidth + HorizontalOffset : 0;
        public float preferredHeight => Text != null ? Text.preferredHeight + VerticalOffset : 0;
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
            if (Text != null)
            {
                Text.ForceMeshUpdate();   
            }
        }

        public void CalculateLayoutInputVertical()
        {
            if (Text != null)
            {
                Text.ForceMeshUpdate();   
            }
        }
        
        void OnTextChanged(Object obj)
        {
            if (obj.Equals(Text) == false)
            {
                return;
            }

            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform);
        }
    }
}