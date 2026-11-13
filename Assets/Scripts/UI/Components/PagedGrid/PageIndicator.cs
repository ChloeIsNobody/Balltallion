using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Components.PagedGrid
{
    public class PageIndicator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int dotSize = 100;
        
        [SerializeField] private Sprite unfilledDotSprite;
        [SerializeField] private Color unfilledDotColor = Color.white;
        [SerializeField] private float unfilledDotScale = 1.0f;
        [SerializeField] private Sprite filledDotSprite;
        [SerializeField] private Color filledDotColor = Color.white;
        [SerializeField] private float filledDotScale = 1.0f;
        
        [Header("References")]
        [SerializeField] private PagedGridLayout pagedGridLayout;
        [SerializeField] private GameObject dotTemplate;
        private List<Image> dotImages = new();

        private int currentPage;
        
        private void OnValidate()
        {
            dotTemplate.SetActive(false);
        }

        [Button("Apply Settings")]
        private void ApplySettings()
        {
            if (!pagedGridLayout) return;
            RegenerateDotObjects(pagedGridLayout.GetNumPages());
        }

        private void Start()
        {
            if (!pagedGridLayout) return;
            RegenerateDotObjects(pagedGridLayout.GetNumPages());
        }

        private void OnEnable()
        {
            pagedGridLayout.OnPageChanged += OnPageChanged;
        }

        private void OnDisable()
        {
            pagedGridLayout.OnPageChanged -= OnPageChanged;
        }
        
        private void OnPageChanged(int newPage)
        {
            if (newPage >= dotImages.Count) return;
            int oldPage = currentPage;
            currentPage = newPage;
            
            Image oldDot = dotImages[oldPage];
            SetDotFill(oldDot, false);
            
            Image dot = dotImages[currentPage];
            SetDotFill(dot, true);
        }

        private void RegenerateDotObjects(int dotCount)
        {
            List<GameObject> toDestroy = new();
            foreach (Transform child in transform)
            {
                if (child.gameObject != dotTemplate) toDestroy.Add(child.gameObject);
            }
            foreach (GameObject obj in toDestroy) DestroyImmediate(obj);
            dotImages.Clear();
            
            if (!dotTemplate) return;
            for (int i = 0; i < dotCount; i++)
            {
                Image newDot = Instantiate(dotTemplate, transform).GetComponent<Image>();
                SetDotFill(newDot, i == 0);
                
                RectTransform rectTransform = newDot.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(dotSize, dotSize);
                
                newDot.gameObject.SetActive(true);
                dotImages.Add(newDot);
            }
            
            OnPageChanged(pagedGridLayout.GetCurrentPage());
        }

        private void SetDotFill(Image dot, bool filled)
        {
            dot.sprite = filled ? filledDotSprite : unfilledDotSprite;
            dot.color = filled ? filledDotColor : unfilledDotColor;
            float scale = filled ? filledDotScale : unfilledDotScale;
            dot.transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}