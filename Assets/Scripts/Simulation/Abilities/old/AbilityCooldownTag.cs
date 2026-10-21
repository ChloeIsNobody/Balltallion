using System;
using TMPro;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    public class AbilityCooldownTag : MonoBehaviour
    {
        [SerializeField] private ActiveAbility activeAbility;
        [SerializeField] private TextMeshPro cooldownText;

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

        private void Update()
        {
            if (activeAbility.IsOnCooldown())
            {
                cooldownText.text = $"{activeAbility.GetCooldownTimer():F1}s";
            }
        }

        private void OnCooldownEntered()
        {
            
        }

        private void OnAbilityReady()
        {
            cooldownText.text = activeAbility.GetInputName();
        }

        private void OnAbilityActivated()
        {
            cooldownText.text = "";
        }

        private void OnAbilityActivationBlocked()
        {
            
        }
    }
}
