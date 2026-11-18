using UnityEngine;

namespace Balltallion.UI
{
    public static class CanvasGroupUtilities
    {
        public static void SetCanvasGroupEnabled(CanvasGroup canvasGroup, bool enabled)
        {
            if (!canvasGroup) return;
            canvasGroup.interactable = enabled;
            canvasGroup.blocksRaycasts = enabled;
            canvasGroup.alpha = enabled ? 1 : 0;
        }
    }
}