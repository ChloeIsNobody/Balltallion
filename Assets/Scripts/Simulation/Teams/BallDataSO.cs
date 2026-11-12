using System;
using System.Collections.Generic;
using Balltallion.Simulation.Abilities;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Weapons;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace Balltallion.Simulation.Teams
{
    [CreateAssetMenu(menuName = "Balltallion/BallData", fileName = "New Ball")]
    public class BallDataSO : ScriptableObject
    {
        [Header("Display Info")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] public string displayName = "Ball";
        [SerializeField] public Color debugColor = Color.white;
        [SerializeField, ShowAssetPreview, ScriptableObjectIcon] public Sprite ballSprite;
        
        [Header("Loadout")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField, Expandable] public WeaponDataSO weapon;
        [SerializeField] public GameObject activeAbilityPrefab;
        [SerializeField] public List<AbilitySO> innateAbilities;
        
        [Header("Base Stat Values")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] public float abilityCooldown = 5.0f;
        [SerializeField] public int abilityDamage = 1;
        [SerializeField] public float attackCooldown = 3.0f;
        [SerializeField] public int attackDamage = 1;
        [SerializeField, Range(0.0f, 1.75f)] public float bounciness = 1.07f;
        [SerializeField] public int contactDamage;
        [SerializeField, Range(0.0f, 5.0f)] public float gravityScale = 1.0f;
        [SerializeField] public float knockback = 1.0f;
        [SerializeField] public int maxHealth = 100;
        [SerializeField] public float moveSpeed = 7.3f;
        [SerializeField, Range(0.1f, 3.0f)] public float size = 1.0f;
        [SerializeField] public float spinRate = 1.0f;
        [SerializeField, Range(0.0f, 100.0f)] public float toughness = 0.0f;
        
        
        [Header("Physics Properties")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        
        [Space]
        [SerializeField, Range(0.0f, 1.0f)] public float linearDamping;

        [Header("Animation Properties")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField, Range(0.0f, 3.0f)] public float animStiffness = 0.35f;
        [SerializeField, Range(0.0f, 1.0f)] public float animDamping = 0.3f;
        [SerializeField, Range(0.0f, 1.0f)] public float animVelocityStretching = 0.1f;
        [SerializeField, Range(0.0f, 360.0f)] public float animHueShift = 0.0f;
        [SerializeField, Range(0.0f, 2.0f)] public float animSaturation = 1.0f;
        [SerializeField, Range(0.0f, 2.0f)] public float animBrightness = 1.0f;
        [SerializeField, Range(0.0f, 2.0f)] public float animContrast = 1.0f;
        
        [Space]
        [Header("Audio Properties")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] public AudioResource bounceSound;
        [SerializeField] public AudioResource damageSound;

        public float GetBaseStat(BattleBall.StatNames statName)
        {
            switch (statName)
            {
                case BattleBall.StatNames.AbilityCooldown: return abilityCooldown;
                case BattleBall.StatNames.AbilityDamage: return abilityDamage;
                case BattleBall.StatNames.AttackCooldown: return attackCooldown;
                case BattleBall.StatNames.AttackDamage: return attackDamage;
                case BattleBall.StatNames.Bounciness: return bounciness;
                case BattleBall.StatNames.ContactDamage: return contactDamage;
                case BattleBall.StatNames.Gravity: return gravityScale;
                case BattleBall.StatNames.Knockback: return knockback;
                case BattleBall.StatNames.MaxHealth: return maxHealth;
                case BattleBall.StatNames.MoveSpeed: return moveSpeed;
                case BattleBall.StatNames.Size: return size;
                case BattleBall.StatNames.SpinRate: return spinRate;
                case BattleBall.StatNames.Toughness: return toughness;
                default:
                    Debug.LogError($"StatName {statName.ToString()} does not have a default value defined in BallDataSO!");
                    break;
            }
            return 0.0f;
        }
    }
    
    // Marks a Sprite field to be used as the icon for this ScriptableObject in the Project view.
    // Only one field per ScriptableObject should have this attribute.
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ScriptableObjectIconAttribute : PropertyAttribute {}
}
