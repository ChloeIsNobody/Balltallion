using Balltallion.DebugTools;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.BallSimulation
{
    public class BallAnimator : MonoBehaviour
    {
        [SerializeField] private BallBody ballBody;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Velocity Squash & Stretch")] 
        [SerializeField] private bool enableVelocityStretching;
        [SerializeField, MinMaxSlider(0.0f, 50.0f)] private Vector2 velocityRange;
        [SerializeField, MinMaxSlider(0.0f, 3.0f)] private Vector2 velocityStretchRange;
        [SerializeField, Range(0.1f, 5.0f)] private float velocityStretchExponent;
        [SerializeField, Range(0.0f, 30.0f)] private float velocityRestoreRate = 0.05f;

        [Header("Collision Squash & Stretch")]
        [SerializeField] private bool enableCollisionStretching;
        [SerializeField, Range(0.0f, 2.0f)] private float collisionPowerX = 1.0f;
        [SerializeField, Range(0.0f, 2.0f)] private float collisionPowerY = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)] private float springStiffness = 0.2f;
        [SerializeField, Range(0.0f, 1.0f)] private float springDamping = 0.2f;
        
        private Material material;
        private Spring springX;
        private Spring springY;

        private float velocityStretchFactor;

        private void Awake()
        {
            material = spriteRenderer.material;
            springX = new Spring(springStiffness, springDamping, 1.0f, 0.25f, 2.0f);
            springX.Reset();
            
            springY = new Spring(springStiffness, springDamping, 1.0f, 0.25f, 2.0f);
            springY.Reset();
            
            velocityStretchFactor = 1.0f;
        }

        private void OnEnable()
        {
            ballBody.OnBounce += OnCollision;
            ballBody.OnBallCollision += OnCollision;
        }

        private void OnDisable()
        {
            ballBody.OnBounce -= OnCollision;
            ballBody.OnBallCollision -= OnCollision;
        }

        private void Update()
        {
            if (enableVelocityStretching)
            {
                VelocityStretching();
                velocityStretchFactor = Mathf.Clamp01(velocityStretchFactor + Time.deltaTime * velocityRestoreRate);
            }

            springX.Update(Time.deltaTime);
            springY.Update(Time.deltaTime);
            material.SetFloat("_Stretch2X", springX.GetPosition());
            material.SetFloat("_Stretch2Y", springY.GetPosition());
        }
        
        private void VelocityStretching()
        {
            Vector2 velocity = ballBody.GetVelocity();
            float t = Mathf.InverseLerp(velocityRange.x, velocityRange.y, velocity.magnitude);
            t = Mathf.Pow(t, velocityStretchExponent);
            float stretch = Mathf.Lerp(velocityStretchRange.x, velocityStretchRange.y, t * velocityStretchFactor);
            
            float stretchAngle = -Vector2.SignedAngle(velocity, Vector2.right);
            
            material.SetFloat("_Stretch1Angle", stretchAngle);
            material.SetFloat("_Stretch1X", stretch);
            material.SetFloat("_Stretch1Y", 1.0f/stretch);
        }

        private void OnCollision(BallCollisionData collisionData)
        {
            if (!enableCollisionStretching) return;
            
            springX.Reset();
            springY.Reset();
            springX.SetPosition(1.0f - collisionPowerX * collisionData.relativeCollisionPower);
            springY.NudgeVelocity(collisionPowerY * collisionData.relativeCollisionPower);
            
            float stretchAngle = -Vector2.SignedAngle(collisionData.contactNormal, Vector2.right);
            material.SetFloat("_Stretch2Angle", stretchAngle);
            
            velocityStretchFactor = 0.0f;
        }
    }
}