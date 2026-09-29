using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.BallSimulation
{
    public class BallAnimator : MonoBehaviour
    {
        [SerializeField] private BallBody ballBody;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("Velocity Squash & Stretch")]
        [SerializeField, MinMaxSlider(0.0f, 50.0f)] private Vector2 velocityRange;
        [SerializeField, MinMaxSlider(0.0f, 3.0f)] private Vector2 velocityStretchRange;
        [SerializeField, Range(0.1f, 5.0f)] private float velocityStretchExponent;

        [Header("Collision Squash & Stretch")]
        [SerializeField, Range(0.0f, 10.0f)] private float springPowerAdjustment = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)] private float springStiffness = 0.2f;
        [SerializeField, Range(0.0f, 1.0f)] private float springDamping = 0.2f;
        
        private Material material;
        private Spring collisionSpring;

        private void Awake()
        {
            material = spriteRenderer.material;
            collisionSpring = new Spring(springStiffness, springDamping);
            collisionSpring.SetGoalPosition(1.0f);
            collisionSpring.Reset();
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
            VelocityStretching();
            
        }

        private void FixedUpdate()
        {
            collisionSpring.Update(Time.fixedDeltaTime);
            material.SetFloat("_Stretch2X", 1.0f/collisionSpring.GetPosition());
            material.SetFloat("_Stretch2Y", collisionSpring.GetPosition());
        }
        
        private void VelocityStretching()
        {
            Vector2 velocity = ballBody.GetVelocity();
            float t = Mathf.InverseLerp(velocityRange.x, velocityRange.y, velocity.magnitude);
            t = Mathf.Pow(t, velocityStretchExponent);
            float stretch = Mathf.Lerp(velocityStretchRange.x, velocityStretchRange.y, t);
            
            float stretchAngle = -Vector2.SignedAngle(velocity, Vector2.right);
            
            material.SetFloat("_Stretch1Angle", stretchAngle);
            material.SetFloat("_Stretch1X", stretch);
            material.SetFloat("_Stretch1Y", 1.0f/stretch);
        }

        private void OnCollision(BallCollisionData collisionData)
        {
            collisionSpring.Reset();
            //collisionSpring.NudgeVelocity(collisionData.collisionPower * springPowerAdjustment);
            collisionSpring.SetPosition(1.0f + springPowerAdjustment* collisionData.relativeCollisionPower);
            
            float stretchAngle = -Vector2.SignedAngle(collisionData.contactNormal, Vector2.right);
            material.SetFloat("_Stretch2Angle", stretchAngle);
        }
    }
}