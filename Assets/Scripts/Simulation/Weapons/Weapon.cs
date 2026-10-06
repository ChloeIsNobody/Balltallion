using Balltallion.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider;
        private BattleBall parentBall;
        
        private WeaponDataSO weaponData;
        
        public void LoadWeaponData(WeaponDataSO weaponData)
        {
            this.weaponData = weaponData;
            if (weaponData == null) return;
            ResizeWeapon();
        }
        
        public void SetParentBall(BattleBall parentBall) => this.parentBall = parentBall;

        private void ResizeWeapon()
        {
            float colliderX = weaponData.relativeColliderSize.x * weaponData.weaponSprite.texture.width / 256.0f;
            float colliderY = weaponData.relativeColliderSize.y * weaponData.weaponSprite.texture.height / 256.0f;
            boxCollider.size = new Vector2(colliderX, colliderY);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out BattleBall ball))
            {
                if (parentBall.GetTeam() == TeamNames.Player)
                {
                    Debug.Log($"Sword hit {ball.name}");
                }
            }
            else if (other.TryGetComponent(out Weapon weapon))
            {
                if (parentBall.GetTeam() == TeamNames.Player)
                {
                    Debug.Log($"Sword hit {weapon.name}");
                }
            }
        }
    }
}