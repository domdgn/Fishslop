using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DebugPanel.Behaviours
{
    public sealed class ScrollToSelected : MonoBehaviour
    {
        public ScrollRect ScrollRect;  
        public RectTransform Content;  
        public RectTransform Viewport;

        GameObject _previousSelected;
        
        void Update()
        {
            if (EventSystem.current == null)
            {
                return;
            }
            
            // Get the currently selected GameObject
            GameObject selected = EventSystem.current.currentSelectedGameObject;

            if (_previousSelected == selected)
            {
                return;
            }

            _previousSelected = selected;

            if (selected == null || !selected.transform.IsChildOf(Content.transform))
            {
                return;
            }
            
            RectTransform selectedRectTransform = selected.GetComponent<RectTransform>();

            // Check if the selected element is outside the viewport
            if (!IsVisibleInScrollRect(selectedRectTransform))
            {
                // Scroll to bring the selected element into view
                FocusOnItem(ScrollRect, selectedRectTransform);
            }
        }
        
        // Method to check if an element is visible in the ScrollView
        private bool IsVisibleInScrollRect(RectTransform target)
        {
            // Convert the target element's local position to the viewport's local position
            Vector3[] targetCorners = new Vector3[4];
            target.GetWorldCorners(targetCorners);

            Vector3[] viewportCorners = new Vector3[4];
            Viewport.GetWorldCorners(viewportCorners);

            // Check if the target is within the visible area of the ScrollRect
            return targetCorners[2].y <= viewportCorners[2].y && targetCorners[0].y >= viewportCorners[0].y;
        }
        
        static Vector2 CalculateFocusedScrollPosition(ScrollRect scrollView, Vector2 focusPoint)
        {
            Vector2 contentSize = scrollView.content.rect.size;
            Vector2 viewportSize = ((RectTransform)scrollView.content.parent).rect.size;
            Vector2 contentScale = scrollView.content.localScale;

            contentSize.Scale(contentScale);
            focusPoint.Scale(contentScale);

            Vector2 scrollPosition = scrollView.normalizedPosition;

            if (scrollView.horizontal && contentSize.x > viewportSize.x)
            {
                scrollPosition.x =
                    Mathf.Clamp01((focusPoint.x - viewportSize.x * 0.5f) / (contentSize.x - viewportSize.x));
            }

            if (scrollView.vertical && contentSize.y > viewportSize.y)
            {
                scrollPosition.y =
                    Mathf.Clamp01((focusPoint.y - viewportSize.y * 0.5f) / (contentSize.y - viewportSize.y));
            }

            return scrollPosition;
        }

        Vector2 CalculateFocusedScrollPosition(ScrollRect scrollView, RectTransform item)
        {
            Vector2 itemCenterPoint =
                scrollView.content.InverseTransformPoint(item.transform.TransformPoint(item.rect.center));

            Vector2 contentSizeOffset = scrollView.content.rect.size;
            contentSizeOffset.Scale(scrollView.content.pivot);

            return CalculateFocusedScrollPosition(scrollView, itemCenterPoint + contentSizeOffset);
        }

        void FocusAtPoint(ScrollRect scrollView, Vector2 focusPoint)
        {
            scrollView.normalizedPosition = CalculateFocusedScrollPosition(scrollView, focusPoint);
        }

        void FocusOnItem(ScrollRect scrollView, RectTransform item)
        {
            scrollView.normalizedPosition = CalculateFocusedScrollPosition(scrollView, item);
        }
    }
}