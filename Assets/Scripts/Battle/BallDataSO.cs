using System;
using Balltallion.Simulation.Weapons;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Battle
{
    [CreateAssetMenu(menuName = "Balltallion/BallData", fileName = "BallData")]
    public class BallDataSO : ScriptableObject
    {
        [Header("Display Info")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] public string displayName = "Ball";
        [SerializeField] public Color debugColor = Color.white;
        [SerializeField, ShowAssetPreview, ScriptableObjectIcon] public Sprite ballSprite;
        
        [Header("Stats")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField, Range(10, 1000)] public int maxHealth = 100;
        
        [SerializeField] public int contactDamage = 3;
        [SerializeField, Space] public bool velocityScaledContactDamage;
        [SerializeField, EnableIf("VelocityScaling")] private Vector2 velocityScalingRange;
        [SerializeField, EnableIf("VelocityScaling")] private Vector2 contactDamageRange;

        [Space]
        [SerializeField] public WeaponDataSO weapon;
        
        [Header("Physics Properties")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField, Range(0.05f, 5.0f)] public float size = 1.0f;
        [SerializeField, Range(0.05f, 10.0f)] public float mass = 1.0f;
        [SerializeField, Range(0.0f, 5.0f)] public float gravityScale = 1.0f;
        
        [Space]
        [SerializeField, Range(0.0f, 100.0f)] public float speed;
        [SerializeField, Range(0.8f, 2.0f)] public float bounciness = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)] public float linearDamping;

        [Header("Animation Properties")]
        [SerializeField, Range(0.0f, 3.0f)] public float animStiffness = 0.35f;
        [SerializeField, Range(0.0f, 1.0f)] public float animDamping = 0.3f;
        [SerializeField, Range(0.0f, 1.0f)] public float animVelocityStretching = 0.1f;

        public int GetVelocityScaledDamage(float velocity)
        {
            if (velocity < velocityScalingRange.x) return contactDamage;
            float t = Mathf.InverseLerp(velocityScalingRange.x, velocityScalingRange.y, velocity);
            t = Mathf.Clamp01(t);
            float damage = Mathf.Lerp(contactDamageRange.x, contactDamageRange.y, t);
            return Mathf.RoundToInt(damage);
        }
        
        private bool VelocityScaling() => velocityScaledContactDamage;
    }
    
    // Marks a Sprite field to be used as the icon for this ScriptableObject in the Project view.
    // Only one field per ScriptableObject should have this attribute.
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ScriptableObjectIconAttribute : PropertyAttribute {}
}
