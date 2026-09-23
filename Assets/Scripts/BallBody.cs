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
        DebugPanel.Instance.SetDebugLabel(0, "KE", GetKineticEnergy().ToString("F2"));
        DebugPanel.Instance.SetDebugLabel(1, "PE", GetPotentialEnergy().ToString("F2"));
        DebugPanel.Instance.SetDebugLabel(2, "TE", GetTotalEnergy().ToString("F2"));
    }

    private float GetKineticEnergy() => 0.5f*rb.mass*rb.linearVelocity.sqrMagnitude;
    private float GetPotentialEnergy() => rb.mass*(Physics2D.gravity.magnitude*rb.gravityScale)*(rb.position.y - floorHeight);
    private float GetTotalEnergy() => GetKineticEnergy() + GetPotentialEnergy();
}
