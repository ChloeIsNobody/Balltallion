using UnityEngine;

public class BallBody : MonoBehaviour
{
    [SerializeField] private float startForceX;
    [SerializeField] private float startForceY;
    [SerializeField] private float floorHeight;
    [SerializeField, Range(0.5f, 5.0f)] private float size;

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
        
    }
}
