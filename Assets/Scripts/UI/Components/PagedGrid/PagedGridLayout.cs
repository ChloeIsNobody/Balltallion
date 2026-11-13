using System;
using Balltallion.UI.Components.Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Components.PagedGrid
{
    public class PagedGridLayout : MonoBehaviour
    {
        public event Action<int> OnPageChanged;
        
        [Header("Settings")]
        [SerializeField] private LayoutModes layoutMode = LayoutModes.Horizontal;
        [SerializeField] private int pageRows;
        [SerializeField] private int pageColumns;
        [SerializeField] private float scrollSpeed = 10.0f;
        
        [Space]
        [SerializeField] private Vector2Int cellSize = new (100, 100);
        [SerializeField] private int cellSpacing = 35;
        [SerializeField] private int padding = 35;
        
        [Header("References")]
        [SerializeField] private RectTransform resizeTransform;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private GridLayoutGroup gridLayout;

        private int currentPageIdx = 0;
        private float pageLerpVal = 0;
        
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

        private void OnEnable()
        {
            NavigationManager.Instance.OnObjectSelected += OnObjectSelected;
        }

        private void OnDisable()
        {
            NavigationManager.Instance.OnObjectSelected -= OnObjectSelected;   
        }

        private void Update()
        {
            pageLerpVal = Mathf.Lerp(pageLerpVal, currentPageIdx, Time.deltaTime * scrollSpeed);
            gridLayout.padding = GetPaddingValueForPage(pageLerpVal);
        }

        private void OnObjectSelected(Selectable currentSelection)
        {
            int selectablePageIdx = GetSelectablePageIdx(currentSelection);
            SetPageInteraction(selectablePageIdx >= 0);
            if (selectablePageIdx == -1) return;
            if (selectablePageIdx != currentPageIdx) GoToPage(selectablePageIdx);
        }

        private void SetLayoutMode(LayoutModes newLayoutMode)
        {
            layoutMode = newLayoutMode;
            gridLayout.startAxis = newLayoutMode == LayoutModes.Horizontal ? GridLayoutGroup.Axis.Vertical : GridLayoutGroup.Axis.Horizontal;
            gridLayout.constraint = newLayoutMode == LayoutModes.Horizontal ? GridLayoutGroup.Constraint.FixedRowCount : GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = newLayoutMode == LayoutModes.Horizontal ? pageRows : pageColumns;
            GoToPage(currentPageIdx);
        }
        
        private void GoToPage(int newPageIdx, bool immediate = false)
        {
            if (immediate) pageLerpVal = currentPageIdx;
            if (currentPageIdx == newPageIdx) return;
            currentPageIdx = newPageIdx;
            OnPageChanged?.Invoke(currentPageIdx);
        }

        private RectOffset GetPaddingValueForPage(float pageIdx)
        {
            int defaultPadding = (padding + cellSpacing) / 2;
            RectOffset result = new RectOffset(defaultPadding, 0, defaultPadding, 0);
            if (layoutMode == LayoutModes.Horizontal) result.left -= (int)(pageIdx * GetPageOffset(layoutMode));
            if (layoutMode == LayoutModes.Vertical) result.top -= (int)(pageIdx * GetPageOffset(layoutMode));
            return result;
        }

        private int GetPageOffset(LayoutModes mode)
        {
            if (mode == LayoutModes.Horizontal) return pageColumns * (cellSize.x + cellSpacing);
            if (mode == LayoutModes.Vertical) return pageRows * (cellSize.y + cellSpacing);
            return 0;
        }
        
        private void SetPageInteraction(bool interactable)
        {
            foreach (Transform child in transform)
            {
                Selectable selectable = child.GetComponent<Selectable>();
                int pageIdx = GetSelectablePageIdx(selectable);
                selectable.interactable = pageIdx == currentPageIdx || interactable;
            }
        }
        
        private int GetSelectablePageIdx(Selectable selectable)
        {
            int selectableIdx = GetSelectableIdx(selectable);
            if (selectableIdx == -1) return -1;
            
            int elementsPerPage = pageRows * pageColumns;
            return selectableIdx / elementsPerPage;
        }

        private int GetSelectableIdx(Selectable selectable)
        {
            if (selectable.transform.IsChildOf(transform)) return selectable.transform.GetSiblingIndex();
            return -1;
        }

        public int GetCurrentPage() => currentPageIdx;
        public int GetNumPages() => Mathf.Max(1, 1 + (transform.childCount-1) / (pageRows * pageColumns));
    }
}