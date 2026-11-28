using TMPro;
using UnityEngine;

namespace Balltallion.Simulation.ActiveAbilities
{
    public class AbilityCooldownTag : MonoBehaviour
    {
        [SerializeField] private ActiveAbility activeAbility;
        [SerializeField] private TextMeshPro cooldownText;
        [SerializeField] private SpriteRenderer iconSpriteRenderer;

        private void Awake()
        {
            iconSpriteRenderer.sprite = null;
        }
        
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
                cooldownText.text = $"{activeAbility.GetSecondsUntilReady():F1}s";
            }
            
            if (!iconSpriteRenderer.sprite) iconSpriteRenderer.sprite = activeAbility.GetAbilityIcon();
        }

        private void OnCooldownEntered()
        {
            cooldownText.enabled = true;
            iconSpriteRenderer.enabled = false;
        }

        private void OnAbilityReady()
        {
            cooldownText.enabled = false;
            iconSpriteRenderer.enabled = true;
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
