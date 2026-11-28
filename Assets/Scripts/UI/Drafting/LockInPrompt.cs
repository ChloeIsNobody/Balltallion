using System;
using Balltallion.Players;
using Balltallion.UI.Components;
using Balltallion.UI.Components.Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class LockInPrompt : MonoBehaviour
    {
        public event Action OnLockedIn;
        
        [SerializeField] private float timeToLockIn;
        [SerializeField] private float timeToDecay;
        [SerializeField] private float progressBarSensitivity = 2.0f;
        
        [Header("Style")]
        [SerializeField] private int backgroundPaletteIdx = 0;
        [SerializeField] private int progressBarPaletteIdx = 0;
        [SerializeField] private Color backgroundInactiveColor;
        [SerializeField] private Color inputIconActiveColor;
        [SerializeField] private Color inputIconInactiveColor;

        [Header("References")]
        [SerializeField] private ContextualInputPrompt inputPrompt;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image progressBarImage;
        [SerializeField] private Image inputIconImage;
        
        private LocalPlayerSO targetPlayer;
        private float lockInProgress;

        private bool isActive = false;
        
        private void OnEnable()
        {
            NavigationManager.Instance.OnPlayerTurnStarted += SetTargetPlayer;
        }

        private void OnDisable()
        {
            NavigationManager.Instance.OnPlayerTurnStarted -= SetTargetPlayer;
        }

        private void Awake()
        {
            SetActive(false);
        }

        private void Update()
        {
            if (inputPrompt && inputPrompt.PollActionIsInProgress())
            {
                lockInProgress = Mathf.Clamp01(lockInProgress += Time.deltaTime /  timeToLockIn);
                
                if (lockInProgress >= 1f)
                {
                    OnLockedIn?.Invoke();
                    lockInProgress = 0f;
                }
            }
            else
            {
                lockInProgress = Mathf.Clamp01(lockInProgress -= Time.deltaTime / timeToDecay);
            }
            
            progressBarImage.fillAmount = LerpEaseOut(lockInProgress, progressBarSensitivity);
        }

        private float LerpEaseOut(float x, float power) => 1.0f - Mathf.Pow(1.0f - x, power);

        public void SetTargetPlayer(LocalPlayerSO newTargetPlayer)
        {
            targetPlayer = newTargetPlayer;
            if (targetPlayer.IsConnected()) UpdatePromptForPlayer(targetPlayer);
        }

        public void SetActive(bool value)
        {
            isActive = value;
            backgroundImage.color = isActive ? GetBackgroundActiveColor() : backgroundInactiveColor;
            inputIconImage.color =  isActive ? inputIconActiveColor : inputIconInactiveColor;
            progressBarImage.enabled = isActive;
        }
        
        private void UpdatePromptForPlayer(LocalPlayerSO player)
        {
            targetPlayer = player;
            if (isActive)
            {
                backgroundImage.color = isActive ? GetBackgroundActiveColor() : backgroundInactiveColor;
                progressBarImage.color = GetProgressBarColor();
            }
        }
        
        private Color GetBackgroundActiveColor() => targetPlayer.GetColorFromPalette(backgroundPaletteIdx);
        private Color GetProgressBarColor() =>  targetPlayer.GetColorFromPalette(progressBarPaletteIdx);
    }
}