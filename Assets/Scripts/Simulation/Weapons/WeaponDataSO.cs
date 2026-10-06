using Balltallion.Battle;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    [CreateAssetMenu(menuName = "Balltallion/WeaponData", fileName = "WeaponData")]
    public class WeaponDataSO : ScriptableObject
    {
        [Header("Display Info")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField, ShowAssetPreview, ScriptableObjectIcon] public Sprite weaponSprite;

        [Header("Stats")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] public int damage;
        [SerializeField] public float knockback;
        [SerializeField, Range(0.0f, 10.0f)] public float spinSpeed;
        [SerializeField] public int weaponCount;

        [Header("Position/Size")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] public Vector2 relativeColliderSize = Vector2.one;
        [SerializeField, Range(0.0f, 3.0f)] public float radialOffset = 1.0f;
        [SerializeField, Range(0.5f, 2.0f)] public float size = 1.0f;
    }
}