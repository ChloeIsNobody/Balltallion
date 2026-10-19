using System;
using Balltallion.Battle;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Balltallion.Simulation
{
    public class BallBody : MonoBehaviour
    {
        public event Action<BallCollisionData> OnWallBounce;
        public event Action<BallCollisionData> OnBounce;
        public event Action<BallCollisionData> OnBallCollision;

        [SerializeField] private GlobalPhysicsDataSO globalPhysicsData;
        [SerializeField] private bool adjustVelocityPostCollision = true; 
        
        [Header("Debug")]
        [SerializeField] private bool debugDraw;
        [SerializeField] private Color debugColor = Color.red;
        
        [Header("LayerMasks")]
        [SerializeField] private LayerMask wallLayerMask;
        [SerializeField] private LayerMask ballLayerMask;

        private Rigidbody2D rb;
        private CircleCollider2D collider;
        private BallDataSO ballData;
        private Vector2 velocityLastFixedUpdate;

        [SerializeField] private float speed = 10.0f;
        [SerializeField] private float bounciness = 1.0f;
        [SerializeField] private float linearDamping;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            collider = GetComponent<CircleCollider2D>();
        }

        public void LoadBallData(BallDataSO ballData)
        {
            this.ballData = ballData;
            transform.localScale = Vector3.one * ballData.size;
            
            if (!rb) rb = GetComponent<Rigidbody2D>();
            rb.mass = ballData.mass;
            rb.gravityScale = ballData.gravityScale;

            speed = ballData.speed;
            bounciness = ballData.bounciness;
            linearDamping = ballData.linearDamping;
            debugColor = ballData.debugColor;
        }

        private void FixedUpdate()
        {
            velocityLastFixedUpdate = rb.linearVelocity;
            
            // Velocity Hard Limit
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, globalPhysicsData.maxVelocity);
            
            // Linear Damping down to target velocity
            if (rb.linearVelocity.magnitude > speed)
            {
                float velocityDiff = rb.linearVelocity.magnitude - speed;
                rb.linearVelocity -= rb.linearVelocity.normalized * (velocityDiff * linearDamping * Time.fixedDeltaTime);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ContactPoint2D contact = collision.GetContact(0);
            
            if (IsLayerIdInMask(collision.gameObject.layer, wallLayerMask))
            {
                ProcessWallCollision(collision);
            }
            else if (IsLayerIdInMask(collision.gameObject.layer, ballLayerMask))
            {
                if (collision.gameObject.TryGetComponent(out BallBody otherBall))
                {
                    ProcessBallCollision(collision, otherBall);
                }
            }
        }

        private void ProcessWallCollision(Collision2D collision)
        {
            ContactPoint2D contact = collision.GetContact(0);
            float collisionPower = Vector2.Dot(velocityLastFixedUpdate, -contact.normal);
            float collisionDot = collisionPower / velocityLastFixedUpdate.magnitude;
            
            OnWallBounce?.Invoke(new BallCollisionData
            {
                otherBall = null,
                contactPoint = contact.point,
                contactNormal = -contact.normal,
                myCollisionPower = collisionPower,
                totalCollisionPower = collisionPower,
            });
            
            rb.linearVelocity += rb.linearVelocity * (bounciness - 1.0f) * collisionDot;
            float angleFromFloor = Vector2.Angle(contact.normal, Vector2.up);
            if (angleFromFloor < 20.0f && rb.linearVelocity.y < globalPhysicsData.minVelocityYAfterFloorBounce)
            {
                float gravityMult = Mathf.Sqrt(Mathf.Max(0.5f, rb.gravityScale));
                rb.linearVelocityY = globalPhysicsData.minVelocityYAfterFloorBounce * gravityMult;
            }
        }

        private void ProcessBallCollision(Collision2D collision, BallBody otherBall)
        {
            ContactPoint2D contact = collision.GetContact(0);
            
            // CollisionPower is positive if the velocity is going towards the other ball, negative if it's going away
            float collisionPowerA = Vector2.Dot(velocityLastFixedUpdate, -contact.normal);
            float collisionPowerB = Vector2.Dot(otherBall.velocityLastFixedUpdate, contact.normal);
                    
            if (collisionPowerA < 0.0f) collisionPowerA = 0.0f;
            else if (collisionPowerB < 0.0f) collisionPowerA += collisionPowerB;
                    
            OnBallCollision?.Invoke(new BallCollisionData
            {
                otherBall = otherBall,
                contactPoint = contact.point,
                contactNormal = -contact.normal,
                myCollisionPower = collisionPowerA,
                totalCollisionPower = Mathf.Max(collisionPowerA, collisionPowerB),
            });
            
            AdjustVelocityPostCollision();
            
            if (debugDraw) DebugDrawLine(contact.point, -contact.normal, collisionPowerA/5.0f);
        }

        public void ApplyKnockback(Vector2 knockback)
        {
            rb.AddForce(knockback, ForceMode2D.Impulse);
            if (adjustVelocityPostCollision) AdjustVelocityPostCollision();
            
            OnBounce?.Invoke(new BallCollisionData
            {
                otherBall = null,
                contactPoint = rb.position,
                contactNormal = knockback.normalized,
                myCollisionPower = knockback.magnitude,
                totalCollisionPower = knockback.magnitude,
            });
            
            if (debugDraw) DebugDrawLine(transform.position, knockback.normalized, knockback.magnitude/5.0f);
        }
        
        
        private void DebugDrawLine(Vector2 point, Vector2 normal, float size)
        {
            Debug.DrawLine(point, point + normal*size, debugColor, 2, false);
        }

        private void AdjustVelocityPostCollision()
        {
            if (Mathf.Abs(rb.linearVelocityX) < globalPhysicsData.minVelocityXAfterCollision)
            {
                rb.linearVelocityX = globalPhysicsData.minVelocityXAfterCollision * Mathf.Sign(rb.linearVelocityX);
            }
            
            if (Mathf.Abs(rb.linearVelocityY) < globalPhysicsData.minVelocityYAfterCollision)
            {
                rb.linearVelocityY = globalPhysicsData.minVelocityYAfterCollision * Mathf.Sign(rb.linearVelocityY);
            }
        }
        private bool IsLayerIdInMask(int layerId, LayerMask mask) => (mask & (1 << layerId)) != 0;
        
        public Vector2 GetVelocity() => rb.linearVelocity;
        public void SetVelocity(Vector2 newVelocity) => rb.linearVelocity = newVelocity;
        public float GetKineticEnergy() => 0.5f * rb.mass * rb.linearVelocity.sqrMagnitude;
        public float GetSpeed() => speed;
        public void SetColliderEnabled(bool enabled) => collider.enabled = enabled;
        public void SetGravityScale(float newGravityScale) => rb.gravityScale = newGravityScale;
        private Vector2 ProjectVector(Vector2 a, Vector2 b) => Vector2.Dot(a, b) * b / b.sqrMagnitude;
    }
}
