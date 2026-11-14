using UnityEngine;

namespace DebugPanel.Behaviours
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaHelper : MonoBehaviour
    {
        public RectTransform RectTransform;

        public bool AffectVertical = true;
        public bool AffectHorizontal = true;
        
        void Start()
        {
            Refresh();
        }

#if UNITY_EDITOR
        Rect _lastSafeArea;
        
        void LateUpdate()
        {
            if (_lastSafeArea != Screen.safeArea)
            {
                _lastSafeArea = Screen.safeArea;
                Refresh();
            }
        }
#endif

        void Refresh()
        {
            Rect safeArea = Screen.safeArea;
            
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            if (AffectHorizontal)
            {
                anchorMin.x /= Screen.width;   
                anchorMax.x /= Screen.width;
            }

            if (AffectVertical)
            {
                anchorMin.y /= Screen.height;   
                anchorMax.y /= Screen.height;
            }

            if (AffectHorizontal)
            {
                RectTransform.anchorMin = new Vector2(anchorMin.x, RectTransform.anchorMin.y);
                RectTransform.anchorMax = new Vector2(anchorMax.x, RectTransform.anchorMax.y);
            }
            
            if (AffectVertical)
            {
                RectTransform.anchorMin = new Vector2(RectTransform.anchorMin.x, anchorMin.y);
                RectTransform.anchorMax = new Vector2(RectTransform.anchorMax.x, anchorMax.y);
            }
        }
    }
}