using Balltallion.Input;
using UnityEngine;

namespace Balltallion.UI.Components.DevicePairing
{
    public class SplitKeyboardPrompt : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        
        private InputManager inputManager;

        private void Start()
        {
            inputManager = InputManager.Instance;
        }
        
        private void Update()
        {
            if (!inputManager) return;
            CanvasGroupUtilities.SetCanvasGroupEnabled(canvasGroup, inputManager.CanActivateSplitKeyboard);
        }
    }
}