using System.Collections;
using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Physics;
using Balltallion.Simulation.Projectiles;
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
        [SerializeField] private BallAnimator ballAnimator;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioSource bounceAudioSource;
        
        [Header("Shield Stats")]
        [SerializeField] private float radius;
        [SerializeField] private float duration;
        [SerializeField] private float popKnockback;

        [Header("Animation")]
        [SerializeField] private float inflateAnimTime;
        [SerializeField] private float deflateAnimTime;

        [Header("Audio")]
        [SerializeField] private AudioResource inflateSound;
        [SerializeField] private AudioResource deflateSound;
        [SerializeField] private AudioResource popSound;
        [SerializeField] private AudioResource bounceSound;
        
        private CircleCollider2D ballCollider;
        private BallBody ballBody;

        private bool popTriggered;
        private bool isShieldActive;
        
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
            ballBody = parentBall.GetComponent<BallBody>();
            ballAnimator.SetTrackedBody(ballBody);
            
            InitializeStateMachine(ShieldState, nameof(ShieldState));
        }

        private void OnEnable()
        {
            ballBody.OnWallBounce += OnWallBounce;
        }

        private void OnDisable()
        {
            ballBody.OnWallBounce -= OnWallBounce;
        }
        
        protected override void CooldownEntered()
        {
            isShieldActive = false;
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
            isShieldActive = true;
            parentBall.SetInvulnerable(true);
            triggerCollider.enabled = true;
            ballCollider.radius = radius;
            animator.speed = 1.0f/inflateAnimTime;
            animator.Play("Inflate");
            PlaySound(inflateSound);

            float shieldTimer = duration;
            while (shieldTimer > 0.0f)
            {
                shieldTimer -= Time.deltaTime;
                if (popTriggered) break;
                yield return null;
            }

            isShieldActive = false;
            parentBall.SetInvulnerable(false);
            ballCollider.radius = 0.5f;
            
            if (!popTriggered)
            {
                PlaySound(deflateSound);
                animator.speed = 1.0f/deflateAnimTime;
                animator.Play("Deflate");
                yield return new WaitForSeconds(deflateAnimTime);
            }
            
            TransitionToCooldown();
        }

        public void Pop()
        {
            popTriggered = true;
            triggerCollider.enabled = false;
            PlaySound(popSound);
        }

        private void OnWallBounce(BallCollisionData data)
        {
            if (isShieldActive)
            {
                bounceAudioSource.Stop();
                bounceAudioSource.resource = bounceSound;
                bounceAudioSource.Play();
            }
        }

        private void PlaySound(AudioResource sound)
        {
            audioSource.Stop();
            audioSource.resource = sound;
            audioSource.Play();
        }
        
        public float GetPopKnockback() => popKnockback;
        public bool IsShieldActive() => isShieldActive;
    }
}