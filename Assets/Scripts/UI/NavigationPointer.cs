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

        private void Update()
        {
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            crosshairImage.enabled = selectedObject;
            if (selectedObject)
            {
                crosshairImage.transform.position = selectedObject.transform.position;
                Rect rect = selectedObject.GetComponent<RectTransform>().rect;
                crosshairImage.rectTransform.sizeDelta = new Vector2(rect.width + paddingX, rect.height + paddingY);
            }
        }
    }
}