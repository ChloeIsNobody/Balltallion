using Balltallion.BallSimulation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.DebugTools
{
    public class TempBallAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField, Range(0.0f, 3.0f)] private float springStiffness = 0.2f;
        [SerializeField, Range(0.0f, 1.0f)] private float springDamping = 0.2f;

        [SerializeField, Range(-10.0f, 10.0f)] private float nudgeValue = 1.0f;
        
        private Spring animSpring;
        private Material material;

        private void OnValidate()
        {
            if (animSpring != null)
            {
                animSpring.SetStiffness(springStiffness);
                animSpring.SetDamping(springDamping);
            }
        }
        
        private void Awake()
        {
            material = spriteRenderer.material;
            animSpring = new Spring(springStiffness, springDamping, 1.0f, 0.5f, 2.0f);
            animSpring.Reset();
        }

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                animSpring.NudgeVelocity(nudgeValue);
            }
        }

        private void FixedUpdate()
        {
            animSpring.Update(Time.fixedDeltaTime);
            material.SetFloat("_Stretch3X", 1.0f/animSpring.GetPosition());
            material.SetFloat("_Stretch3Y", animSpring.GetPosition());
        }
    }
}