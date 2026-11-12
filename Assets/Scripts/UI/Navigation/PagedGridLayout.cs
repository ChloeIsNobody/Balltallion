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

            gridLayout.padding = new RectOffset(padding, padding, padding, padding);
            
            if (layoutMode == LayoutModes.Horizontal)
            {
                int pageOffset = pageColumns * (cellSize.x + cellSpacing);
                gridLayout.padding.left -= currentPageIdx * pageOffset;
            }
            else
            {
                int pageOffset = pageRows * (cellSize.y + cellSpacing);
                gridLayout.padding.top -= currentPageIdx * pageOffset;
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}