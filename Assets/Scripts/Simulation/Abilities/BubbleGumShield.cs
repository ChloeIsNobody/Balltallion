using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Balltallion.Simulation.Abilities
{
    public class BubbleGumShield : ActiveAbility
    {
        [Header("References")] 
        [SerializeField] private CircleCollider2D triggerCollider;
        [SerializeField] private Transform visualTransform;
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource audioSource;
        
        [Header("Shield Stats")]
        [SerializeField] private float radius;
        [SerializeField] private float duration;
        [SerializeField] private float popKnockback;

        [Header("Audio")]
        [SerializeField] private AudioResource inflateSound;
        [SerializeField] private AudioResource deflateSound;
        [SerializeField] private AudioResource popSound;
        
        private CircleCollider2D ballCollider;

        private bool popTriggered;
        
        private void OnValidate()
        {
            if (visualTransform == null || triggerCollider == null) return;
            visualTransform.transform.localScale = new Vector3(radius*2.0f, radius*2.0f, 1.0f);
            triggerCollider.radius = radius;
        }

        protected override void Awake()
        {
            OnValidate();
            base.Awake();
            ballCollider = parentBall.GetComponent<CircleCollider2D>();
            InitializeStateMachine(ShieldState, nameof(ShieldState));
        }
        
        protected override void CooldownEntered()
        {
            parentBall.SetInvulnerable(false);
            triggerCollider.enabled = false;
            ballCollider.radius = 0.5f;
            animator.Play("Idle");
            popTriggered = false;
        }

        protected override void AbilityReady()
        {
            
        }

        private IEnumerator ShieldState()
        {
            parentBall.SetInvulnerable(true);
            triggerCollider.enabled = true;
            ballCollider.radius = radius;
            animator.Play("Active");
            PlaySound(inflateSound);

            float shieldTimer = duration;
            while (shieldTimer > 0.0f)
            {
                shieldTimer -= Time.deltaTime;
                if (popTriggered) break;
                yield return null;
            }

            if (!popTriggered) PlaySound(deflateSound);
            
            TransitionToCooldown();
        }

        public void Pop()
        {
            popTriggered = true;
            PlaySound(popSound);
        }

        private void PlaySound(AudioResource sound)
        {
            audioSource.Stop();
            audioSource.resource = sound;
            audioSource.Play();
        }
        
        public float GetPopKnockback() => popKnockback;
    }
}