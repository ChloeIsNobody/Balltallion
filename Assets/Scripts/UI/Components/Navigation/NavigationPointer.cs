using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Balltallion.UI.Components.Navigation
{
    public class NavigationPointer : MonoBehaviour
    {
        [SerializeField] private NavigationManager navigationManager;
        [SerializeField] private Image crosshairImage;
        [SerializeField] private float paddingX;
        [SerializeField] private float paddingY;
        [SerializeField] private float lerpSpeed = 1.0f;
        
        private RectTransform rectTransform;
        
        private Selectable targetSelectable;
        
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

        private void OnEnable()
        {
            navigationManager.OnObjectSelected += UpdateTargetSelectable;
        }

        private void OnDisable()
        {
            navigationManager.OnObjectSelected -= UpdateTargetSelectable;
        }

        private void Update()
        {
            MoveSelectionIfNotInteractable();
            crosshairImage.enabled = targetSelectable;
            LerpPointer();
        }

        private void UpdateTargetSelectable(Selectable newSelectable)
        {
            if (newSelectable == targetSelectable) return;
            if (!newSelectable)
            {
                targetSelectable = null;
                return;
            }
            
            Selectable oldSelectable = targetSelectable;
            targetSelectable = newSelectable;
                
            oldLerpState = GetCurrentLerpState();
            targetLerpState = new LerpState
            {
                position = GetSelectablePosition(newSelectable),
                size = GetSelectableSize(newSelectable)
            };
            if (!oldSelectable) oldLerpState = targetLerpState;
            
            lerpVal = 0.0f;
        }

        private void MoveSelectionIfNotInteractable()
        {
            if (!targetSelectable) return;
            if (!targetSelectable.IsInteractable() || !targetSelectable.isActiveAndEnabled)
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
            if (!targetSelectable) return;
            
            targetLerpState.position = GetSelectablePosition(targetSelectable);
            targetLerpState.size = GetSelectableSize(targetSelectable);
            
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
        
        private Vector2 GetSelectablePosition(Selectable a) =>a.GetComponent<RectTransform>().position;
        private Vector2 GetSelectableSize(Selectable a) => a.GetComponent<RectTransform>().rect.size + new Vector2(paddingX, paddingY);
        
        public void SetColor(Color newColor) => crosshairImage.color = newColor;
    }
}