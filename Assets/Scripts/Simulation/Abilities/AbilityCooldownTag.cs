using System;
using TMPro;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    public class AbilityCooldownTag : MonoBehaviour
    {
        [SerializeField] private ActiveAbility activeAbility;
        [SerializeField] private TextMeshPro cooldownText;
        [SerializeField] private SpriteRenderer iconSpriteRenderer;

        private void OnEnable()
        {
            activeAbility.OnCooldownEntered += OnCooldownEntered;
            activeAbility.OnAbilityReady += OnAbilityReady;
            activeAbility.OnAbilityActivated += OnAbilityActivated;
            activeAbility.OnAbilityActivationBlocked += OnAbilityActivationBlocked;
        }

        private void OnDisable()
        {
            activeAbility.OnCooldownEntered -= OnCooldownEntered;
            activeAbility.OnAbilityReady -= OnAbilityReady;
            activeAbility.OnAbilityActivated -= OnAbilityActivated;
            activeAbility.OnAbilityActivationBlocked -= OnAbilityActivationBlocked;
        }

        private void Start()
        {
            iconSpriteRenderer.enabled = false;
        }

        private void Update()
        {
            if (activeAbility.IsOnCooldown())
            {
                cooldownText.text = $"{activeAbility.GetCooldownTimer():F1}s";
            }
        }

        private void OnCooldownEntered()
        {
            cooldownText.enabled = true;
        }

        private void OnAbilityReady()
        {
            cooldownText.enabled = false;
            iconSpriteRenderer.enabled = true;
            iconSpriteRenderer.sprite = activeAbility.GetAbilityIcon();
        }

        private void OnAbilityActivated()
        {
            cooldownText.text = "";
            iconSpriteRenderer.enabled = false;
        }

        private void OnAbilityActivationBlocked()
        {
            
        }
    }
}
