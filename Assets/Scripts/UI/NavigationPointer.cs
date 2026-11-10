using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Balltallion.UI
{
    public class NavigationPointer : MonoBehaviour
    {
        [SerializeField] private Image crosshairImage;
        [SerializeField] private float paddingX;
        [SerializeField] private float paddingY;
        [SerializeField] private float lerpSpeed = 1.0f;
        
        private RectTransform rectTransform;

        private GameObject currentSelectedObject;
        
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
            GameObject newSelectedObject = EventSystem.current.currentSelectedGameObject;
            crosshairImage.enabled = currentSelectedObject;

            if (!newSelectedObject)
            {
                currentSelectedObject = null;
                return;
            } 
            
            if (newSelectedObject != currentSelectedObject)
            {
                currentSelectedObject = newSelectedObject;
                
                oldLerpState = GetCurrentLerpState();
                targetLerpState = new LerpState
                {
                    position = GetObjectPosition(newSelectedObject),
                    size = GetObjectSize(newSelectedObject)
                };
                lerpVal = 0.0f;
            }

            LerpPointer();
        }

        private void LerpPointer()
        {
            if (!currentSelectedObject) return;
            
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
    }
}