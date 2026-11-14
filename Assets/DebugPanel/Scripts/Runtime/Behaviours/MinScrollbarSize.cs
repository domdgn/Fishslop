using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Behaviours
{
    public sealed class MinScrollbarSize : MonoBehaviour
    {
        public Scrollbar Scrollbar;
        [Range(0, 1)] public float MinSize = 0.1f;
        
        void LateUpdate() 
        {
            if (Scrollbar == null)
            {
                return;
            }
            
            if (Scrollbar.size < MinSize) 
            {
                Scrollbar.size = MinSize;
            }
        }
    }
}