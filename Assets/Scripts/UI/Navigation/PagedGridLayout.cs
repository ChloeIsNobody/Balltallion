using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Navigation
{
    public class PagedGridLayout : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayoutModes layoutMode = LayoutModes.Horizontal;
        [SerializeField] private int pageRows;
        [SerializeField] private int pageColumns;
        
        [Space]
        [SerializeField] private Vector2Int cellSize = new (100, 100);
        [SerializeField] private int cellSpacing = 35;
        [SerializeField] private int padding = 35;

        [Header("Debug")]
        [SerializeField] private int debugSetPageIdx = 0;
        
        [Header("References")]
        [SerializeField] private RectTransform resizeTransform;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private GridLayoutGroup gridLayout;

        private int currentPageIdx = 0;
        
        private enum LayoutModes
        {
            Horizontal,
            Vertical
        }

        private void OnValidate()
        {
            if (!gridLayout) return;
            gridLayout.cellSize = cellSize;
            gridLayout.spacing = cellSpacing * Vector2.one;

            if (resizeTransform)
            {
                Vector2 sizeDelta = new Vector2(GetPageOffset(LayoutModes.Horizontal),
                    GetPageOffset(LayoutModes.Vertical));
                sizeDelta += padding * Vector2.one;
                resizeTransform.sizeDelta = sizeDelta;
            }
            
            SetLayoutMode(layoutMode);
        }
        
        private void Awake()
        {
            
        }

        [Button("SetPageIdx")]
        private void DebugSetPageIdx()
        {
            SetPageIdx(debugSetPageIdx);
        }

        private void SetLayoutMode(LayoutModes newLayoutMode)
        {
            layoutMode = newLayoutMode;
            gridLayout.startAxis = newLayoutMode == LayoutModes.Horizontal ? GridLayoutGroup.Axis.Vertical : GridLayoutGroup.Axis.Horizontal;
            gridLayout.constraint = newLayoutMode == LayoutModes.Horizontal ? GridLayoutGroup.Constraint.FixedRowCount : GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = newLayoutMode == LayoutModes.Horizontal ? pageRows : pageColumns;
            SetPageIdx(currentPageIdx);
        }
        
        private void SetPageIdx(int newPageIdx)
        {
            currentPageIdx = newPageIdx;
            gridLayout.padding = GetPaddingValueForPage(currentPageIdx);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private RectOffset GetPaddingValueForPage(int pageIdx)
        {
            int defaultPadding = (padding + cellSpacing) / 2;
            RectOffset result = new RectOffset(defaultPadding, 0, defaultPadding, 0);
            if (layoutMode == LayoutModes.Horizontal) result.left -= pageIdx * GetPageOffset(layoutMode);
            if (layoutMode == LayoutModes.Vertical) result.top -= pageIdx * GetPageOffset(layoutMode);
            return result;
        }

        private int GetPageOffset(LayoutModes mode)
        {
            if (mode == LayoutModes.Horizontal) return pageColumns * (cellSize.x + cellSpacing);
            if (mode == LayoutModes.Vertical) return pageRows * (cellSize.y + cellSpacing);
            return 0;
        }
    }
}