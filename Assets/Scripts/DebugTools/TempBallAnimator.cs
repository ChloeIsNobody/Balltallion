using Balltallion.BallSimulation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.DebugTools
{
    public class TempBallAnimator
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField, Range(0.0f, 1.0f)] private float springStiffness = 0.2f;
        [SerializeField, Range(0.0f, 1.0f)] private float springDamping = 0.2f;

        [SerializeField, Range(-10.0f, 10.0f)] private float nudgeValue = 1.0f;
        
        private Spring animSpring;
        private Material material;
        
        private void Awake()
        {
            material = spriteRenderer.material;
            animSpring = new Spring(springStiffness, springDamping);
            animSpring.SetGoalPosition(1.0f);
            animSpring.Reset();
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                animSpring.NudgeVelocity(nudgeValue);
            }
        }
    }
}