using System;
using Balltallion.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Navigation
{
    public class MenuTab : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LocalPlayerSO targetPlayer;
        [SerializeField] private string actionPath;
        
        [Space]
        [SerializeField] private CanvasGroup menuCanvasGroup;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;
        
        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] private Image tabImage;
        [SerializeField] private ContextualInputPrompt inputPrompt;

        private PlayerInputRelaySO inputRelay;
        private Action selectionRequestBinding;

        private void Awake()
        {
            inputPrompt.SetTargetPlayer(targetPlayer);
            inputPrompt.SetActionPath(actionPath);
            inputRelay = targetPlayer.GetInputRelay();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }

        private void OnButtonClicked()
        {
            selectionRequestBinding?.Invoke();
        }

        private void Update()
        {
            if (!inputRelay.IsConnected() || !button.IsInteractable()) return;
            if (inputRelay.PollActionPerformedThisFrame(actionPath))
            {
                selectionRequestBinding?.Invoke();
            }
        }
        
        public void BindSelectionRequest(Action action) => selectionRequestBinding = action;

        public void Select()
        {
            CanvasGroupUtilities.SetCanvasGroupEnabled(menuCanvasGroup, true);
            tabImage.color = activeColor;
        }

        public void Deselect()
        {
            CanvasGroupUtilities.SetCanvasGroupEnabled(menuCanvasGroup, false);
            tabImage.color = inactiveColor;
        }
    }
}