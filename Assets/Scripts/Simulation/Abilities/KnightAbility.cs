using System;
using System.Collections;
using Balltallion.Simulation.Battle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.Simulation.Abilities
{
    public class KnightAbility : MonoBehaviour
    {
        public event Action OnCooldownComplete;
        
        private BattleBall parentBall;

        [SerializeField] private float cooldown = 1.0f;
        
        private float cooldownTimer;
        private bool onCooldown;
        
        private Coroutine cooldownCoroutine;

        private void Awake()
        {
            parentBall = GetComponentInParent<BattleBall>();
        }

        private void Start()
        {
            StartCooldown();
        }
        
        private void Update()
        {
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (CanActivateAbility())
                {
                    Activate();
                }
                else
                {
                    Debug.Log($"{parentBall.name}'s ability is on cooldown! ({cooldownTimer:F1}s left...)");
                }
            }
        }

        private bool CanActivateAbility() => cooldownTimer <= 0.0f;

        private void Activate()
        {
            Debug.Log($"{parentBall.name}'s ability activated!");
            StartCooldown();
        }

        private void StartCooldown()
        {
            if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        }
        
        private IEnumerator CooldownCoroutine()
        {
            onCooldown = true;
            cooldownTimer = cooldown;

            while (cooldownTimer > 0.0f)
            {
                cooldownTimer -= Time.deltaTime;
                yield return null;
            }
            
            cooldownTimer = 0.0f;
            onCooldown = false;
            
            Debug.Log($"{parentBall.name}'s ability is now ready!");
            OnCooldownComplete?.Invoke();
        }
    }
}