using UnityEngine;
using UnityEngine.Serialization;

namespace DebugPanel.Examples
{
    public sealed class DebugPanelExampleField : MonoBehaviour
    {
        public enum FieldBackgroundColor
        {
            Green,
            Blue,
            Black,
        }
        
        public SpriteRenderer BackgroundSpriteRenderer;

        public FieldBackgroundColor BackgroundColor = FieldBackgroundColor.Black;
        public Color YellowColor;
        public Color BlueColor;
        public Color BlackColor;
        
        public void SetColor(FieldBackgroundColor fieldColor)
        {
            Color color = YellowColor;
            
            switch (fieldColor)
            {
                case FieldBackgroundColor.Green:
                    color = YellowColor;
                    break;
                case FieldBackgroundColor.Blue:
                    color = BlueColor;
                    break;
                case FieldBackgroundColor.Black:
                    color = BlackColor;
                    break;
            }

            BackgroundSpriteRenderer.color = color;
            BackgroundColor = fieldColor;
        }
    }
}