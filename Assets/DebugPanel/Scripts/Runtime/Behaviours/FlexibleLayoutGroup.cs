#nullable enable

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DebugPanel.Behaviours
{
    /// <summary>
    /// Layout Group controller that arranges children in rows, fitting as many on a line until total width exceeds parent
    /// bounds
    /// </summary>
    public class FlexibleLayoutGroup : LayoutGroup
    {
        public bool ChildForceExpandHeight;
        public bool ChildForceExpandWidth;
        public float Spacing;
        public bool UseParentAsWorkingWidth;
        
        float _layoutWidth;
        float _layoutHeight;
        
        readonly List<RectTransform> _rowList = new();
        
        protected bool IsCenterAlign 
            => childAlignment is TextAnchor.LowerCenter or TextAnchor.MiddleCenter or TextAnchor.UpperCenter;

        protected bool IsRightAlign 
            => childAlignment is TextAnchor.LowerRight or TextAnchor.MiddleRight or TextAnchor.UpperRight;

        protected bool IsMiddleAlign 
            => childAlignment is TextAnchor.MiddleLeft or TextAnchor.MiddleRight or TextAnchor.MiddleCenter;

        protected bool IsLowerAlign 
            => childAlignment is TextAnchor.LowerLeft or TextAnchor.LowerRight or TextAnchor.LowerCenter;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            float calculatedMinWidth = GetGreatestMinimumChildWidth() + padding.left + padding.right;

            SetLayout(rectTransform.rect.width, 0, false);
            
            SetLayoutInputForAxis(calculatedMinWidth, _layoutWidth, -1, 0);
        }

        public override void SetLayoutHorizontal()
        {
            SetLayout(rectTransform.rect.width, 0, false);
        }

        public override void SetLayoutVertical()
        {
            SetLayout(rectTransform.rect.width, 1, false);
        }

        public override void CalculateLayoutInputVertical()
        {
            _layoutHeight = SetLayout(rectTransform.rect.width, 1, true);
        }

        /// <summary>
        /// Main layout method
        /// </summary>
        /// <param name="width">Width to calculate the layout with</param>
        /// <param name="axis">0 for horizontal axis, 1 for vertical</param>
        /// <param name="layoutInput">If true, sets the layout input for the axis. If false, sets child position for axis</param>
        public float SetLayout(float width, int axis, bool layoutInput)
        {
            float groupHeight = rectTransform.rect.height;

            RectTransform widthRectToUse = rectTransform;
            float extraWidthPadding = 0;

            if (UseParentAsWorkingWidth)
            {
                RectTransform? parent = FindLimitingParent(rectTransform);

                if (parent != null)
                {
                    widthRectToUse = parent;
                    LayoutGroup? parentLayoutGroup = parent.GetComponent<LayoutGroup>();

                    if (parentLayoutGroup != null)
                    {
                        extraWidthPadding += parentLayoutGroup.padding.left + parentLayoutGroup.padding.right;
                    }      
                }
            }
            
            // Width that is available after padding is subtracted
            float workingWidth = widthRectToUse.rect.width - padding.left - padding.right - extraWidthPadding;

            // Accumulates the total height of the rows, including spacing and padding.
            float yOffset = IsLowerAlign ? padding.bottom : (float)padding.top;

            float currentRowWidth = 0f;
            float currentRowHeight = 0f;

            _layoutWidth = 0;
            
            for (int i = 0; i < rectChildren.Count; i++)
            {
                // LowerAlign works from back to front
                int index = IsLowerAlign ? rectChildren.Count - 1 - i : i;

                RectTransform? child = rectChildren[index];

                float childWidth = LayoutUtility.GetPreferredSize(child, 0);
                float childHeight = LayoutUtility.GetPreferredSize(child, 1);

                // Max child width is layout group with - padding
                childWidth = Mathf.Min(childWidth, workingWidth);

                // Apply spacing if not the first element in a row
                if (_rowList.Count > 0)
                {
                    currentRowWidth += Spacing;
                }
                
                // If adding this element would exceed the bounds of the row,
                // go to a new line after processing the current row
                if (currentRowWidth + childWidth > workingWidth)
                {
                    // Undo spacing addition if we're moving to a new line (Spacing is not applied on edges)
                    currentRowWidth -= Spacing;

                    // Process current row elements positioning
                    if (!layoutInput)
                    {
                        float h = CalculateRowVerticalOffset(groupHeight, yOffset, currentRowHeight);
                        LayoutRow(_rowList, currentRowWidth, currentRowHeight, workingWidth, padding.left, h, axis);
                    }

                    // Clear existing row
                    _rowList.Clear();

                    // Add the current row height to total height accumulator, and reset to 0 for the next row
                    yOffset += currentRowHeight;
                    yOffset += Spacing;

                    currentRowHeight = 0;
                    currentRowWidth = 0;
                }

                currentRowWidth += childWidth;
                _rowList.Add(child);
                
                _layoutWidth = Mathf.Max(_layoutWidth, currentRowWidth);

                // We need the largest element height to determine the starting position of the next line
                if (childHeight > currentRowHeight)
                {
                    currentRowHeight = childHeight;
                }
            }

            _layoutWidth += padding.left + padding.right;

            if (!layoutInput)
            {
                float h = CalculateRowVerticalOffset(groupHeight, yOffset, currentRowHeight);

                // Layout the final row
                LayoutRow(_rowList, currentRowWidth, currentRowHeight, workingWidth, padding.left, h, axis);
            }

            _rowList.Clear();

            // Add the last rows height to the height accumulator
            yOffset += currentRowHeight;
            yOffset += IsLowerAlign ? padding.top : padding.bottom;

            if (layoutInput)
            {
                if (axis == 1)
                {
                    SetLayoutInputForAxis(yOffset, yOffset, -1, axis);
                }
            }

            return yOffset;
        }

        RectTransform? FindLimitingParent(RectTransform rect)
        {
            Transform current = rect.parent;
            while (current != null)
            {
                RectTransform? parentRect = current as RectTransform;
                
                if (parentRect == null)
                {
                    break;
                }

                LayoutGroup layoutElement = parentRect.GetComponent<LayoutGroup>();
                
                if (layoutElement != null) 
                {
                    return parentRect;
                }

                current = current.parent;
            }

            return null; // No limiting parent found, assume full screen size
        }
    
        
        private float CalculateRowVerticalOffset(float groupHeight, float yOffset, float currentRowHeight)
        {
            float h;

            if (IsLowerAlign)
            {
                h = groupHeight - yOffset - currentRowHeight;
            }
            else if (IsMiddleAlign)
            {
                h = groupHeight * 0.5f - _layoutHeight * 0.5f + yOffset;
            }
            else
            {
                h = yOffset;
            }
            return h;
        }

        protected void LayoutRow(
            IList<RectTransform> contents, 
            float rowWidth, 
            float rowHeight, 
            float maxWidth,
            float xOffset, 
            float yOffset, 
            int axis
            )
        {
            float xPos = xOffset;

            if (!ChildForceExpandWidth && IsCenterAlign)
            {
                xPos += (maxWidth - rowWidth) * 0.5f;
            }
            else if (!ChildForceExpandWidth && IsRightAlign)
            {
                xPos += (maxWidth - rowWidth);
            }

            float extraWidth = 0f;

            if (ChildForceExpandWidth)
            {
                int flexibleChildCount = 0;

                for (int i = 0; i < _rowList.Count; i++)
                {
                    if (LayoutUtility.GetFlexibleWidth(_rowList[i]) > 0f)
                    {
                        flexibleChildCount++;
                    }
                }

                if (flexibleChildCount > 0)
                {
                    extraWidth = (maxWidth - rowWidth) / flexibleChildCount;
                }
            }

            for (int j = 0; j < _rowList.Count; j++)
            {
                int index = IsLowerAlign ? _rowList.Count - 1 - j : j;

                RectTransform? rowChild = _rowList[index];

                float rowChildWidth = LayoutUtility.GetPreferredSize(rowChild, 0);

                if (LayoutUtility.GetFlexibleWidth(rowChild) > 0f)
                {
                    rowChildWidth += extraWidth;
                }

                float rowChildHeight = LayoutUtility.GetPreferredSize(rowChild, 1);

                if (ChildForceExpandHeight)
                {
                    rowChildHeight = rowHeight;
                }

                rowChildWidth = Mathf.Min(rowChildWidth, maxWidth);

                float yPos = yOffset;

                if (IsMiddleAlign)
                {
                    yPos += (rowHeight - rowChildHeight) * 0.5f;
                }
                else if (IsLowerAlign)
                {
                    yPos += (rowHeight - rowChildHeight);
                }

                if (axis == 0)
                {
#if UNITY_2019_1
                    SetChildAlongAxis(rowChild, 0, 1f, xPos, rowChildWidth);
#else
                    SetChildAlongAxis(rowChild, 0, xPos, rowChildWidth);
#endif
                }
                else
                {
#if UNITY_2019_1
                    SetChildAlongAxis(rowChild, 1, 1f, yPos, rowChildHeight);
#else
                    SetChildAlongAxis(rowChild, 1, yPos, rowChildHeight);
#endif
                }

                xPos += rowChildWidth + Spacing;
            }
        }

        public float GetGreatestMinimumChildWidth()
        {
            float max = 0f;

            for (var i = 0; i < rectChildren.Count; i++)
            {
                float w = LayoutUtility.GetMinWidth(rectChildren[i]);

                max = Mathf.Max(w, max);
            }

            return max;
        }
    }
}