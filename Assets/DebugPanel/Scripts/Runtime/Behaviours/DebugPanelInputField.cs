using TMPro;
using UnityEngine.EventSystems;

namespace DebugPanel.Behaviours
{
    public sealed class DebugPanelInputField : TMP_InputField
    {
        public override void OnSelect(BaseEventData eventData)
        {
            TMP_Text previousText = m_TextComponent;
            
            m_TextComponent = null;
            
            base.OnSelect(eventData);
            
            m_TextComponent = previousText;
        }
    }
}