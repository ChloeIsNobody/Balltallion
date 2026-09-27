using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion
{
    public class DebugBallAttacker : MonoBehaviour
    {
        [SerializeField] private BattleBall targetBall;
        [SerializeField] private int damage;
        
        [SerializeField] private float knockbackStrength;
        [SerializeField] private Vector2 knockbackDirection;
        [SerializeField] private KnockbackMode knockbackMode;

        private enum KnockbackMode
        {
            Fixed,
            Random,
            AwayFromCursor,
            TowardsCursor,
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                AttackData attackData = new();
                attackData.source = null;
                attackData.damage = damage;
                attackData.knockback = knockbackStrength * GetKnockbackDirection();
                targetBall?.Attack(attackData);
            }
        }

        private Vector2 GetKnockbackDirection()
        {
            if (knockbackMode == KnockbackMode.Fixed) return knockbackDirection.normalized;
            if (knockbackMode == KnockbackMode.Random) return Random.onUnitCircle;
            if (knockbackMode == KnockbackMode.AwayFromCursor)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                return ((Vector2)targetBall.transform.position - mousePos).normalized;
            }
            if (knockbackMode == KnockbackMode.TowardsCursor)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                return (mousePos - (Vector2)targetBall.transform.position).normalized;
            }

            return Vector2.zero;
        }
    }
}