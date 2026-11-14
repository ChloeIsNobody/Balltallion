using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Components.PagedGrid
{
    public class PageArrows : MonoBehaviour
    {
        [SerializeField] private PagedGridLayout pagedGridLayout;
        [SerializeField] private Image leftArrow;
        [SerializeField] private Image rightArrow;

        private void OnEnable()
        {
            pagedGridLayout.OnPageChanged += OnPageChanged;
            pagedGridLayout.OnFocusChanged += OnFocusChanged;
        }

        private void OnDisable()
        {
            pagedGridLayout.OnPageChanged -= OnPageChanged;
            pagedGridLayout.OnFocusChanged -= OnFocusChanged;
        }

        private void Start()
        {
            UpdateArrowVisibility();
        }

        private void OnFocusChanged(bool hasFocus)
        {
            UpdateArrowVisibility();
        }

        private void OnPageChanged(int newPage)
        {
            UpdateArrowVisibility();
        }

        private void UpdateArrowVisibility()
        {
            leftArrow.enabled = pagedGridLayout.HasFocus() && !pagedGridLayout.IsAtLeftMostPage();
            rightArrow.enabled = pagedGridLayout.HasFocus() && !pagedGridLayout.IsAtRightMostPage();
        }
    }
}