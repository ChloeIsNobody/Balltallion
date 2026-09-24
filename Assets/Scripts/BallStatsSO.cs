using System;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion
{
    [CreateAssetMenu(menuName = "Balltallion/Balls", fileName = "BallStats")]
    public class BallStatsSO : ScriptableObject
    {
        [Header("Display Info")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] public string displayName = "Ball";
        [SerializeField, ShowAssetPreview, ScriptableObjectIcon] public Sprite ballSprite;
        
        
        [Header("Stats")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField, Range(10, 1000)] public int maxHealth = 100;
        
        [SerializeField] public bool velocityScaledContactDamage;
        [SerializeField, HideIf("VelocityScaling")] public int contactDamage = 3;
        [SerializeField, ShowIf("VelocityScaling")] private Vector2 velocityScalingRange;
        [SerializeField, ShowIf("VelocityScaling")] private Vector2 contactDamageRange;
        
        
        [Header("Physics Properties")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField, Range(0.05f, 5.0f)] public float size = 1.0f;
        [SerializeField, Range(0.05f, 10.0f)] public float mass = 1.0f;
        [SerializeField, Range(0.0f, 5.0f)] public float gravityScale = 1.0f;

        public int GetVelocityScaledDamage(float velocity)
        {
            if (velocity < velocityScalingRange.x) return 0;
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
