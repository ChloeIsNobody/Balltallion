using Balltallion.Battle;
using Balltallion.Simulation.Projectiles;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace Balltallion.Simulation.Weapons
{
    [CreateAssetMenu(menuName = "Balltallion/WeaponData", fileName = "WeaponData")]
    public class WeaponDataSO : ScriptableObject
    {
        [Header("Display Info")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField, ShowAssetPreview, ScriptableObjectIcon] public Sprite weaponSprite;

        [SerializeField] public int weaponCount;
        [SerializeField] public WeaponAimModes weaponAimMode = WeaponAimModes.Spinning;
        [SerializeField, ShowIf("SpinMode"), Range(0.0f, 10.0f)] public float spinSpeed;
        [SerializeField, ShowIf("StaticMode"), Range(0.0f, 360.0f)] public float staticAimAngle;
        
        [Header("Melee Stats")] 
        [HorizontalLine(color: EColor.Gray, height: 1.5f)]
        [SerializeField] public bool ignoresCollisions;
        [SerializeField, HideIf("IgnoresCollisions")] public int damage;
        [SerializeField, HideIf("IgnoresCollisions")] public float knockback;

        [Header("Ranged Stats")]
        [HorizontalLine(color: EColor.Gray, height: 1.5f)]
        [SerializeField] public bool shootsProjectiles;
        [SerializeField, ShowIf("ShootsProjectiles")] public Projectile projectilePrefab;
        [SerializeField, ShowIf("ShootsProjectiles"), Range(1, 50)] public int ammo = 1;
        [SerializeField, ShowIf("ShootsProjectiles"), Range(0.05f, 1.0f)] public float delayBetweenShots;
        [SerializeField, ShowIf("ShootsProjectiles"), Range(0.05f, 10.0f)] public float reloadDuration = 2.0f;
        [SerializeField, ShowIf("ShootsProjectiles"), Range(0.0f, 360.0f)] public float shotSpread = 0.0f;
        [SerializeField, ShowIf("ShootsProjectiles"), Range(0.0f, 10.0f)] public float shotRecoil = 0.0f;

        [Header("Position/Size")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] public Vector2 relativeColliderSize = Vector2.one;
        [SerializeField, Range(0.0f, 3.0f)] public float radialOffset = 1.0f;
        [SerializeField, Range(0.5f, 2.0f)] public float size = 1.0f;
        
        [Header("Audio Properties")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] public AudioResource attackSound;
        
        private bool IgnoresCollisions() => ignoresCollisions;
        private bool ShootsProjectiles() => shootsProjectiles;
        private bool SpinMode() => weaponAimMode == WeaponAimModes.Spinning;
        private bool StaticMode() => weaponAimMode == WeaponAimModes.Static;
    }
    
    public enum WeaponAimModes
    {
        Spinning,
        Static,
        Following
    }
}