using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Balltallion
{
    public class BallBody : MonoBehaviour
    {
        public event Action<BallCollisionData> OnBounce;
        public event Action<BallCollisionData> OnBallCollision;

        [Header("Physics Settings")]
        [SerializeField, Range(0.0f, 100.0f)] private float targetVelocity;
        [SerializeField, Range(0.0f, 100.0f)] private float maxVelocity;
        [SerializeField, Range(0.0f, 1.0f)] private float linearDamping;
        [SerializeField, Range(0.8f, 2.0f)] private float bounciness = 1.0f;
        [SerializeField, Range(0.0f, 25.0f)] private float minCollisionAngleX;
        [SerializeField, Range(0.0f, 25.0f)] private float minCollisionAngleY;

        [Header("Debug")]
        [SerializeField] private bool debugDraw;
        [SerializeField] private Color debugDrawColor;
        
        [Header("LayerMasks")]
        [SerializeField] private LayerMask wallLayerMask;
        [SerializeField] private LayerMask ballLayerMask;

        [SerializeField] private float startAngleBase;
        [SerializeField] private float startAngleVariation;

        private Rigidbody2D rb;
        private Vector2 velocityLastFixedUpdate;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            float startAngle = startAngleBase + Random.Range(-startAngleVariation, startAngleVariation);
            Vector2 startVelocity = new Vector2(Mathf.Cos(Mathf.Deg2Rad * startAngle), Mathf.Sin(Mathf.Deg2Rad * startAngle)) * targetVelocity;
            rb.linearVelocity = startVelocity;
        }

        public void LoadBallStats(BallStatsSO ballStats)
        {
            transform.localScale = Vector3.one * ballStats.size;
            
            if (!rb) rb = GetComponent<Rigidbody2D>();
            rb.mass = ballStats.mass;
            rb.gravityScale = ballStats.gravityScale;
        }

        private void FixedUpdate()
        {
            velocityLastFixedUpdate = rb.linearVelocity;
            
            // Velocity Hard Limit
            if (rb.linearVelocity.magnitude > maxVelocity)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
            }
            
            // Linear Damping down to target velocity
            if (rb.linearVelocity.magnitude > targetVelocity)
            {
                float velocityDiff = rb.linearVelocity.magnitude - targetVelocity;
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
            
            OnBounce?.Invoke(new BallCollisionData
            {
                otherBall = null,
                contactPoint = contact.point,
                contactNormal = -contact.normal,
                collisionPower = collisionPower
            });
            
            rb.linearVelocity += rb.linearVelocity * (bounciness - 1.0f) * collisionPower / velocityLastFixedUpdate.magnitude;
        }

        private void ProcessBallCollision(Collision2D collision, BallBody otherBall)
        {
            ContactPoint2D contact = collision.GetContact(0);
            
            // CollisionPower is positive if the velocity is going towards the other ball, negative if it's going away
            float collisionPowerA = Vector2.Dot(velocityLastFixedUpdate, -contact.normal);
            float collisionPowerB = Vector2.Dot(otherBall.velocityLastFixedUpdate, contact.normal);
                    
            if (collisionPowerA < 0.0f) collisionPowerA = 0.0f;
            else if (collisionPowerB < 0.0f) collisionPowerA += collisionPowerB;
                    
            //collisionPowerA *= rb.mass;
                    
            OnBallCollision?.Invoke(new BallCollisionData
            {
                otherBall = otherBall,
                contactPoint = contact.point,
                contactNormal = -contact.normal,
                collisionPower = collisionPowerA
            });
            
            AdjustVelocityPostCollision();
                    
            if (debugDraw) DebugDrawCollision(contact.point, -contact.normal, collisionPowerA/5.0f, debugDrawColor);
        }

        public void ApplyKnockback(Vector2 knockback)
        {
            rb.AddForce(knockback, ForceMode2D.Impulse);
            AdjustVelocityPostCollision();
        }
        
        private void DebugDrawCollision(Vector2 point, Vector2 normal, float size, Color color)
        {
            Debug.DrawLine(point, point + normal*size, color, 2, false);
        }

        public Vector2 GetVelocity() => rb.linearVelocity;
        public float GetKineticEnergy() => 0.5f * rb.mass * rb.linearVelocity.sqrMagnitude;
        
        private Vector2 ProjectVector(Vector2 a, Vector2 b) => Vector2.Dot(a, b) * b / b.sqrMagnitude;

        private void AdjustVelocityPostCollision()
        {
            float angleX = GetAngleFromHorizontal(rb.linearVelocity);
            float angleY = GetAngleFromVertical(rb.linearVelocity);
            
            float adjustAmount = 0.0f;
            if (angleX < minCollisionAngleX) adjustAmount = minCollisionAngleX - angleX;
            else if (angleY < minCollisionAngleY) adjustAmount = minCollisionAngleY - angleY;
            
            bool xPositive = rb.linearVelocity.x > 0.0f;
            bool yPositive = rb.linearVelocity.y > 0.0f;
            bool yGreater = rb.linearVelocity.y > rb.linearVelocity.x;
            bool adjustClockwise = xPositive ^ yPositive ^ yGreater; // This symbol is XOR
            if (adjustClockwise) adjustAmount *= -1.0f;
            
            rb.linearVelocity = Quaternion.Euler(0.0f, 0.0f, adjustAmount) * rb.linearVelocity;
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxVelocity);
        }
        private float GetAngleFromHorizontal(Vector2 dir)
        {
            float angle1 = Vector2.Angle(dir, Vector2.right);
            float angle2 = Vector2.Angle(dir, Vector2.left);
            return Mathf.Min(angle1, angle2);
        }
        private float GetAngleFromVertical(Vector2 dir)
        {
            float angle1 = Vector2.Angle(dir, Vector2.up);
            float angle2 = Vector2.Angle(dir, Vector2.down);
            return Mathf.Min(angle1, angle2);
        }

        private bool IsLayerIdInMask(int layerId, LayerMask mask) => (mask & (1 << layerId)) != 0;
    }
}
