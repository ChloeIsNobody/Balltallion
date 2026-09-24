using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Balltallion
{
    public class BallBody : MonoBehaviour
    {
        public event Action<BallCollisionData> OnBounce;
        public event Action<BallCollisionData> OnBallCollision;

        [Header("Debug")]
        [SerializeField] private bool debugDraw;
        [SerializeField] private Color debugDrawColor;
        
        [Header("LayerMasks")]
        [SerializeField] private LayerMask wallLayerMask;
        [SerializeField] private LayerMask ballLayerMask;

        [SerializeField] private float startAngleBase;
        [SerializeField] private float startAngleVariation;
        [SerializeField] private float startForce;
        [SerializeField, Range(0.5f, 5.0f)] private float size = 1.0f;
        [SerializeField, Range(0.2f, 10.0f)] private float mass = 1.0f;
        [SerializeField, Range(0.0f, 5.0f)] private float gravityScale = 1.0f; 

        private Rigidbody2D rb;
        private Vector2 velocityLastFixedUpdate;

        private void OnValidate()
        {
            transform.localScale = Vector3.one * size;
        }
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            float startAngle = startAngleBase + Random.Range(-startAngleVariation, startAngleVariation);
            Vector2 force = new Vector2(Mathf.Cos(Mathf.Deg2Rad * startAngle), Mathf.Sin(Mathf.Deg2Rad * startAngle)) * startForce;
            rb.AddForce(force, ForceMode2D.Impulse);
            
            rb.mass = mass;
            rb.gravityScale = gravityScale;
        }

        private void FixedUpdate()
        {
            rb.mass = mass;
            rb.gravityScale = gravityScale;
            velocityLastFixedUpdate = rb.linearVelocity;
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
            float collisionPowerA = Vector2.Dot(velocityLastFixedUpdate, -contact.normal);
            
            OnBounce?.Invoke(new BallCollisionData
            {
                otherBall = null,
                contactPoint = contact.point,
                contactNormal = -contact.normal,
                collisionPower = collisionPowerA
            });
        }

        private void ProcessBallCollision(Collision2D collision, BallBody otherBall)
        {
            ContactPoint2D contact = collision.GetContact(0);
            
            // CollisionPower is positive if the velocity is going towards the other ball, negative if it's going away
            float collisionPowerA = Vector2.Dot(velocityLastFixedUpdate, -contact.normal);
            float collisionPowerB = Vector2.Dot(otherBall.velocityLastFixedUpdate, contact.normal);
                    
            if (collisionPowerA < 0.0f) collisionPowerA = 0.0f;
            else if (collisionPowerB < 0.0f) collisionPowerA += collisionPowerB;
                    
            collisionPowerA *= rb.mass;
                    
            OnBallCollision?.Invoke(new BallCollisionData
            {
                otherBall = otherBall,
                contactPoint = contact.point,
                contactNormal = -contact.normal,
                collisionPower = collisionPowerA
            });
                    
            if (debugDraw) DebugDrawCollision(contact.point, -contact.normal, collisionPowerA/5.0f);
        }

        private void DebugDrawCollision(Vector2 point, Vector2 normal, float size = 1.0f)
        {
            Debug.DrawLine(point, point + normal*size, debugDrawColor, 2, false);
        }

        private Vector2 ProjectVector(Vector2 a, Vector2 b) => Vector2.Dot(a, b) * b / b.sqrMagnitude;

        private bool IsLayerIdInMask(int layerId, LayerMask mask) => (mask & (1 << layerId)) != 0;
    }
}
