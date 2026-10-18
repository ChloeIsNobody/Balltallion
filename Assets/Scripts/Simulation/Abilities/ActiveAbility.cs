using System;
using System.Collections;
using Balltallion.Simulation.Battle;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Balltallion.Simulation.Abilities
{
    public abstract class ActiveAbility : MonoBehaviour
    {
        public event Action OnCooldownEntered;
        public event Action OnAbilityReady;
        public event Action OnAbilityActivated;
        public event Action OnAbilityActivationBlocked;
        
        [Header("Base Ability Data")]
        [SerializeField] private float cooldown = 1.0f;
        
        protected BattleBall parentBall;
        private KeyControl abilityKey;

        protected TeamNames team;
        private bool onCooldown;
        private float cooldownTimer;

        private Coroutine currentState;
        private string currentStateName;
        
        private Func<IEnumerator> abilityStartState;
        private string abilityStartStateName;

        protected virtual void Awake()
        {
            parentBall = GetComponentInParent<BattleBall>();
            SetupTeam();
        }
        
        private void SetupTeam()
        {
            team = parentBall.GetTeam();
            gameObject.layer = TeamUtilities.GetWeaponTeamLayerID(team);
            abilityKey = parentBall.GetTeam() == TeamNames.Player ? Keyboard.current.dKey : Keyboard.current.jKey;
        }

        protected void InitializeStateMachine(Func<IEnumerator> abilityStartState, string abilityStartStateName)
        {
            this.abilityStartState = abilityStartState;
            this.abilityStartStateName = abilityStartStateName;
            TransitionToState(CooldownState(), nameof(CooldownState));
        }
        
        private IEnumerator CooldownState()
        {
            cooldownTimer = cooldown;
            OnCooldownEntered?.Invoke();

            while (cooldownTimer > 0.0f)
            {
                cooldownTimer -= Time.deltaTime;
                if (abilityKey.wasPressedThisFrame) OnAbilityActivationBlocked?.Invoke();
                yield return null;
            }
            
            cooldownTimer = 0.0f;
            OnAbilityReady?.Invoke();
            
            TransitionToState(ReadyState(), nameof(ReadyState));
        }

        private IEnumerator ReadyState()
        {
            yield return new WaitUntil(() => abilityKey.wasPressedThisFrame);
            OnAbilityActivated?.Invoke();
            
            TransitionToState(abilityStartState.Invoke(), abilityStartStateName);
        }

        protected void TransitionToCooldown() => TransitionToState(CooldownState(), nameof(CooldownState));

        protected void TransitionToState(IEnumerator newStateCoroutine, string newStateName = "Unnamed State")
        {
            if (currentState != null) StopCoroutine(currentState);
            currentState = StartCoroutine(newStateCoroutine);
            Debug.Log($"Transitioning from {currentStateName} to {newStateName}");
            currentStateName = newStateName;
        }
        
        public bool IsOnCooldown() => currentStateName == nameof(CooldownState);
        public float GetCooldownTimer() => cooldownTimer;
    }
}