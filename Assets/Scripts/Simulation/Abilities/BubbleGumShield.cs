using System.Collections;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    public class BubbleGumShield : ActiveAbility
    {
        [Header("References")]
        [SerializeField] private Transform visualTransform;
        [SerializeField] private Animator animator;
        
        [Header("Shield Stats")]
        [SerializeField] private float radius;
        [SerializeField] private float duration;
        
        private CircleCollider2D ballCollider;
        
        private void OnValidate()
        {
            if (visualTransform == null) return;
            visualTransform.transform.localScale = new Vector3(radius*2.0f, radius*2.0f, 1.0f);
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
            ballCollider.radius = 0.5f;
            animator.Play("Idle");
        }

        protected override void AbilityReady()
        {
            
        }

        private IEnumerator ShieldState()
        {
            parentBall.SetInvulnerable(true);
            ballCollider.radius = radius;
            animator.Play("Active");
            
            yield return new WaitForSeconds(duration);
            TransitionToCooldown();
        }
    }
}