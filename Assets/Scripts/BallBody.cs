using System;
using UnityEngine;

public class BallBody : MonoBehaviour
{
    public event Action OnBounce;
    public event Action<BallBody> OnBallCollision;

    [Header("Debug")]
    [SerializeField] private bool debugDraw;
    [SerializeField] private Color debugDrawColor;
    
    [Header("LayerMasks")]
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask ballLayerMask;
    
    [SerializeField] private float startForceX;
    [SerializeField] private float startForceY;
    [SerializeField, Range(0.5f, 5.0f)] private float size = 1.0f;
    [SerializeField, Range(0.2f, 10.0f)] private float mass = 1.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float gravityScale = 1.0f; 

    private Rigidbody2D rb;

    private void OnValidate()
    {
        transform.localScale = Vector3.one * size;
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(startForceX, startForceY), ForceMode2D.Impulse);
    }

    private void Update()
    {
        rb.mass = mass;
        rb.gravityScale = gravityScale;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        ContactPoint2D contact = other.GetContact(0);
        
        if (IsLayerIdInMask(other.gameObject.layer, wallLayerMask))
        {
            Debug.Log("Bounce!");
            OnBounce?.Invoke();
        }
        else if (IsLayerIdInMask(other.gameObject.layer, ballLayerMask))
        {
            if (other.gameObject.TryGetComponent(out BallBody ballBody))
            {
                Debug.Log($"Collided with {other.gameObject.name}!");
                OnBallCollision?.Invoke(ballBody);
                if (debugDraw) DebugDrawCollision(contact.point, contact.normal);
            }
        }
    }

    private void DebugDrawCollision(Vector2 point, Vector2 normal)
    {
        Debug.DrawLine(point, point + normal, debugDrawColor, 2, false);
    }

    private bool IsLayerIdInMask(int layerId, LayerMask mask) => (mask & (1 << layerId)) != 0;
}
