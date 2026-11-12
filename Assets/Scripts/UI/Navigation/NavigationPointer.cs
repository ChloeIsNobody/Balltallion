using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Balltallion.UI.Navigation
{
    public class NavigationPointer : MonoBehaviour
    {
        [SerializeField] private Image crosshairImage;
        [SerializeField] private float paddingX;
        [SerializeField] private float paddingY;
        [SerializeField] private float lerpSpeed = 1.0f;
        
        private RectTransform rectTransform;

        private GameObject currentSelectedObject;
        private Selectable currentSelectable;
        
        private LerpState targetLerpState;
        private LerpState oldLerpState;
        private float lerpVal; // 0: old, 1: target
        private struct LerpState
        {
            public Vector2 position;
            public Vector2 size;
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            SetSelectedObject(EventSystem.current.currentSelectedGameObject);
            MoveSelectionIfNotInteractable();
            crosshairImage.enabled = currentSelectedObject;
            LerpPointer();
        }

        private void SetSelectedObject(GameObject newSelectedObject)
        {
            if (newSelectedObject == currentSelectedObject) return;
            if (!newSelectedObject)
            {
                currentSelectedObject = null;
                return;
            }
            
            GameObject oldSelectedObject = currentSelectedObject;
            currentSelectedObject = newSelectedObject;
            currentSelectable = newSelectedObject.GetComponent<Selectable>();
                
            oldLerpState = GetCurrentLerpState();
            targetLerpState = new LerpState
            {
                position = GetObjectPosition(newSelectedObject),
                size = GetObjectSize(newSelectedObject)
            };
            if (!oldSelectedObject) oldLerpState = targetLerpState;
            lerpVal = 0.0f;
        }

        private void MoveSelectionIfNotInteractable()
        {
            if (!currentSelectable) return;
            if (!currentSelectable.IsInteractable() || !currentSelectable.isActiveAndEnabled)
            {
                GameObject nearestSelectable = FindNearestSelectable().gameObject;
                EventSystem.current.SetSelectedGameObject(nearestSelectable);
            } 
        }

        private Selectable FindNearestSelectable()
        {
            List<Selectable> selectables = new List<Selectable>(FindObjectsByType<Selectable>());
            Selectable nearestSelectable = null;
            float nearestDistance = Mathf.Infinity;
            foreach (Selectable selectable in selectables)
            {
                if (!selectable.enabled || !selectable.IsInteractable()) continue;
                float distance = Vector2.Distance(transform.position, selectable.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestSelectable = selectable;
                }
            }

            return nearestSelectable;
        }

        private void LerpPointer()
        {
            if (!currentSelectedObject) return;
            
            if (currentSelectedObject)
            {
                targetLerpState.position = GetObjectPosition(currentSelectedObject);
                targetLerpState.size = GetObjectSize(currentSelectedObject);
            }
            
            lerpVal = Mathf.Lerp(lerpVal, 1.0f, Time.deltaTime * lerpSpeed);
            LerpState currentLerpState = GetCurrentLerpState();
            rectTransform.position = currentLerpState.position;
            rectTransform.sizeDelta = currentLerpState.size;
        }

        private LerpState GetCurrentLerpState()
        {
            LerpState currentLerpState = new()
            {
                position = Vector2.Lerp(oldLerpState.position, targetLerpState.position, lerpVal),
                size = Vector2.Lerp(oldLerpState.size, targetLerpState.size, lerpVal)
            };
            return currentLerpState;
        }
        
        private Vector2 GetObjectPosition(GameObject a) =>a.GetComponent<RectTransform>().position;
        private Vector2 GetObjectSize(GameObject a) => a.GetComponent<RectTransform>().rect.size + new Vector2(paddingX, paddingY);
        
        public void SetColor(Color newColor) => crosshairImage.color = newColor;
    }
}