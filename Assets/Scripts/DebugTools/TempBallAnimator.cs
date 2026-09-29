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
        
        private Spring animSpringX;
        private Spring animSpringY;
        private Material material;

        private void OnValidate()
        {
            if (animSpringX != null)
            {
                animSpringX.SetStiffness(springStiffness);
                animSpringX.SetDamping(springDamping);
            }
            
            if (animSpringY != null)
            {
                animSpringY.SetStiffness(springStiffness);
                animSpringY.SetDamping(springDamping);
            }
        }
        
        private void Awake()
        {
            material = spriteRenderer.material;
            
            animSpringX = new Spring(springStiffness, springDamping, 1.0f, 0.5f, 2.0f);
            animSpringX.Reset();
            
            animSpringY = new Spring(springStiffness, springDamping, 1.0f, 0.5f, 2.0f);
            animSpringY.Reset();
        }

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                animSpringX.NudgeVelocity(-nudgeValue);
                animSpringY.NudgeVelocity(nudgeValue);
            }
        }

        private void FixedUpdate()
        {
            animSpringX.Update(Time.fixedDeltaTime);
            animSpringY.Update(Time.fixedDeltaTime);
            material.SetFloat("_Stretch3X", animSpringX.GetPosition());
            material.SetFloat("_Stretch3Y", animSpringY.GetPosition());
        }
    }
}