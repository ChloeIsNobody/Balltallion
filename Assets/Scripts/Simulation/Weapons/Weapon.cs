using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider;
        
        private WeaponDataSO weaponData;
        
        public void LoadWeaponStats(WeaponDataSO weaponData)
        {
            this.weaponData = weaponData;
            if (weaponData == null) return;
            ResizeWeapon();
        }

        private void ResizeWeapon()
        {
            float colliderX = weaponData.relativeColliderSize.x * weaponData.weaponSprite.texture.width / 256.0f;
            float colliderY = weaponData.relativeColliderSize.y * weaponData.weaponSprite.texture.height / 256.0f;
            boxCollider.size = new Vector2(colliderX, colliderY);
        }
    }
}