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
        [SerializeField, Range(0.1f, 5.0f)] private float velocityStretchExponent;
        [SerializeField, Range(0.0f, 30.0f)] private float collisionFactorRestoreRate = 0.05f;

        [Header("Collision Squash & Stretch")]
        [SerializeField] private bool enableCollisionStretching;
        [SerializeField, MinMaxSlider(0.0f, 50.0f)] private Vector2 animationPowerMappingIn;
        [SerializeField, MinMaxSlider(0.0f, 1.0f)] private Vector2 animationPowerMappingOut;
        [SerializeField, Range(0.0f, 3.0f)] private float springStiffness = 0.2f;
        [SerializeField, Range(0.0f, 1.0f)] private float springDamping = 0.2f;
        
        private Material material;
        private Spring springX;
        private Spring springY;

        private float collisionFactor;
        private float velocityStretchingFactor;
        
        private void Awake()
        {
            material = spriteRenderer.material;
            springX = new Spring(springStiffness, springDamping, 1.0f, 0.25f, 2.0f);
            springX.Reset();
            
            springY = new Spring(springStiffness, springDamping, 1.0f, 0.25f, 2.0f);
            springY.Reset();
            
            collisionFactor = 1.0f;
        }

        public void SetSpringData(float stiffness, float damping, float mass, float velocityStretchingFactor)
        {
            if (springX == null || springY == null) return;
            springX.SetStiffness(stiffness);
            springY.SetStiffness(stiffness);
            springX.SetDamping(damping);
            springY.SetDamping(damping);
            springX.SetMass(mass);
            springY.SetMass(mass);
            this.velocityStretchingFactor = velocityStretchingFactor;
        }
        
        public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;

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
                collisionFactor = Mathf.Clamp01(collisionFactor + Time.deltaTime * collisionFactorRestoreRate);
            }

            springX.Update(Time.deltaTime);
            springY.Update(Time.deltaTime);
            material.SetFloat("_Stretch2X", springX.GetPosition());
            material.SetFloat("_Stretch2Y", springY.GetPosition());
            material.SetFloat("_Stretch2Displacement", collisionFactor);
        }
        
        private void VelocityStretching()
        {
            Vector2 velocity = ballBody.GetVelocity();
            float t = Mathf.InverseLerp(velocityRange.x, velocityRange.y, velocity.magnitude);
            t = Mathf.Pow(t, velocityStretchExponent);
            float stretch = Mathf.Lerp(1.0f, 1.0f + velocityStretchingFactor, t * collisionFactor);
            
            float stretchAngle = -Vector2.SignedAngle(velocity, Vector2.right);
            
            material.SetFloat("_Stretch1Angle", stretchAngle);
            material.SetFloat("_Stretch1X", stretch);
            material.SetFloat("_Stretch1Y", 1.0f/stretch);
        }

        private void OnCollision(BallCollisionData collisionData)
        {
            if (!enableCollisionStretching) return;
            
            float animationPower = GetAnimationPower(collisionData);
            if (animationPower <= 0.0f) return;
            
            springX.Reset();
            springY.Reset();
            springX.SetPosition(1.0f - animationPower);
            springY.NudgeVelocity(animationPower);
            
            float stretchAngle = -Vector2.SignedAngle(collisionData.contactNormal, Vector2.right);
            material.SetFloat("_Stretch2Angle", stretchAngle);
            
            collisionFactor = 0.0f;
        }

        private float GetAnimationPower(BallCollisionData collisionData)
        {
            float t = Mathf.InverseLerp(animationPowerMappingIn.x, animationPowerMappingIn.y, collisionData.totalCollisionPower);
            t /= springX.GetMass();
            if (t <= 0.0f) return 0.0f;
            return Mathf.Lerp(animationPowerMappingOut.x, animationPowerMappingOut.y, t);
        }
    }
}