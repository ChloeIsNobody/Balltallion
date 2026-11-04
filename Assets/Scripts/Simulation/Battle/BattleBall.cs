using System;
using System.Collections.Generic;
using Balltallion.Audio;
using Balltallion.Input;
using Balltallion.Simulation.Abilities;
using Balltallion.Simulation.ActiveAbilities;
using Balltallion.Simulation.Feel;
using Balltallion.Simulation.Physics;
using Balltallion.Simulation.Teams;
using Balltallion.Simulation.Weapons;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Battle
{
    public class BattleBall : MonoBehaviour
    {
        public event Action OnDeath;
        public event Action OnPrimaryAbilityUsed;
        
        [Header("Loaded Data")]
        [SerializeField, Expandable] private BallDataSO ballData;
        [SerializeField] private TeamNames team;
        
        [Header("References")]
        [SerializeField] private WeaponController weaponController;
        [SerializeField] private AbilityBuilder abilityBuilder;
        [SerializeField] private BallAnimator ballAnimator;
        [SerializeField] private BallAudioHandler audioHandler;
        [SerializeField] private Transform abilitiesParent;
        private ActiveAbility primaryActive;
        
        [Header("Debug")]
        [SerializeField] private bool debugLogToConsole = false;
        [SerializeField] private bool debugAttacks;
        
        private float health;
        private BallBody ballBody;
        private PlayerInputRelaySO inputRelay;

        private float damageResidue; // Stores decimal point damage only dealt when it becomes greater than 1

        private Dictionary<StatNames, Stat> stats = new();
        
        public enum StatNames
        {
            AbilityCooldown,
            AbilityDamage,
            AttackCooldown,
            AttackDamage,
            AttackPower,
            Bounciness,
            ContactDamage,
            Deflection,
            Gravity,
            Knockback,
            MaxHealth,
            MoveSpeed,
            Size,
            SpinRate,
            Toughness,
            Vulnerability,
            Weakness,
        }

        private void Awake()
        {
            ballBody = GetComponent<BallBody>();
        }

        [Button("Reload BallDataSO")]
        private void EditorLoadData()
        {
            if (ballData) LoadBallData(ballData, team);
        }

        public void LoadBallData(BallDataSO data, TeamNames team)
        {
            ballData = data;
            InitializeStats();
            
            if (Application.isPlaying) abilityBuilder.BuildAbilities(data.innateAbilities);
            
            health = (int)stats[StatNames.MaxHealth].GetValue();
            
            SetTeam(team);
            name = data.displayName;
            ballAnimator.SetSprite(data.ballSprite);
            
            if (!ballBody) ballBody = GetComponent<BallBody>();
            ballBody.LoadBallData(data);
            ballBody.LoadSizeStat(stats[StatNames.Size]);
            ballBody.LoadGravityStat(stats[StatNames.Gravity]);
            ballBody.LoadBouncinessStat(stats[StatNames.Bounciness]);
            ballBody.LoadSpeedStat(stats[StatNames.MoveSpeed]);
            ballBody.OnWallBounce += OnWallBounce;
            ballBody.OnBallCollision += OnBallCollision;
            
            ballAnimator.LoadBallData(data);
            
            weaponController.gameObject.SetActive(data.weapon);
            weaponController.LoadWeaponData(data.weapon);

            ClearChildAbilities();
            if (data.activeAbilityPrefab)
            {
                primaryActive = Instantiate(data.activeAbilityPrefab, abilitiesParent.transform).GetComponent<ActiveAbility>();
                primaryActive.gameObject.transform.localPosition = Vector3.zero;
                primaryActive.OnAbilityActivated += OnActiveAbilityUsed;
            }

            audioHandler.LoadBallData(data);
        }
        
        private void InitializeStats()
        {
            stats = new Dictionary<StatNames, Stat>
            {
                [StatNames.AbilityCooldown] = new(nameof(StatNames.AbilityCooldown), ballData.GetBaseStat(StatNames.AbilityCooldown), "F1"),
                [StatNames.AbilityDamage] = new(nameof(StatNames.AbilityDamage), ballData.GetBaseStat(StatNames.AbilityDamage), "F0"),
                [StatNames.AttackCooldown] = new(nameof(StatNames.AttackCooldown), ballData.GetBaseStat(StatNames.AttackCooldown), "F1"),
                [StatNames.AttackDamage] = new(nameof(StatNames.AttackDamage), ballData.GetBaseStat(StatNames.AttackDamage), "F0"),
                [StatNames.AttackPower] = new(nameof(StatNames.AttackPower), 1.0f, "F2"),
                [StatNames.Bounciness] = new(nameof(StatNames.Bounciness), ballData.GetBaseStat(StatNames.Bounciness), "F2", 0.0f, 1.75f),
                [StatNames.ContactDamage] = new(nameof(StatNames.ContactDamage), ballData.GetBaseStat(StatNames.ContactDamage), "F0"),
                [StatNames.Deflection] = new(nameof(StatNames.Deflection), 0.0f, "F0%"),
                [StatNames.Gravity] = new(nameof(StatNames.Gravity), ballData.GetBaseStat(StatNames.Gravity), "F2", 0.0f, 5.0f),
                [StatNames.Knockback] = new(nameof(StatNames.Knockback), ballData.GetBaseStat(StatNames.Knockback), "F2"),
                [StatNames.MaxHealth] = new(nameof(StatNames.MaxHealth), ballData.GetBaseStat(StatNames.MaxHealth), "F0"),
                [StatNames.MoveSpeed] = new(nameof(StatNames.MoveSpeed), ballData.GetBaseStat(StatNames.MoveSpeed), "F1"),
                [StatNames.Size] = new(nameof(StatNames.Size), ballData.GetBaseStat(StatNames.Size), "F2", 0.1f, 3.0f),
                [StatNames.SpinRate] = new(nameof(StatNames.SpinRate), ballData.GetBaseStat(StatNames.SpinRate), "F2"),
                [StatNames.Toughness] = new(nameof(StatNames.Toughness), 0.0f, "F0%", 0.0f, 100.0f),
                [StatNames.Vulnerability] = new(nameof(StatNames.Vulnerability), 0.0f, "F0%"),
                [StatNames.Weakness] = new(nameof(StatNames.Weakness), 0.0f, "F0%", 0.0f, 100.0f),
            };
        }
        
        private void ClearChildAbilities()
        {
            List<Transform> abilityTransforms = new();
            foreach (Transform child in abilitiesParent) abilityTransforms.Add(child);
            foreach (Transform child in abilityTransforms) DestroyImmediate(child.gameObject);
        }

        private void SetTeam(TeamNames newTeam)
        {
            team = newTeam;
            gameObject.layer = TeamUtilities.GetTeamLayerID(newTeam);
        }

        public void ReceiveAttack(AttackData attackData)
        {
            Color damageColor = attackData.source?.GetColor() ?? Color.white;

            if (attackData.damage >= 0)
            {
                float finalDamage = CalculateAttackDamage(attackData);
                TakeDamage(attackData.damage, finalDamage, damageColor);
            }

            Vector2 finalKnockback = attackData.knockback;
            if (attackData.source) finalKnockback *= attackData.source.GetStat(StatNames.Knockback).GetValue();
            ballBody.ApplyKnockback(finalKnockback);
            
            if (debugAttacks) DebugAttack(attackData);
        }

        private float CalculateAttackDamage(AttackData attackData)
        {
            float finalDamage = attackData.damage;
            if (attackData.source)
            {
                finalDamage *= attackData.source.GetStat(StatNames.AttackPower).GetValue();
                finalDamage *= 1.0f - attackData.source.GetStat(StatNames.Weakness).GetValue() / 100.0f;
            }
            finalDamage *= 1.0f + stats[StatNames.Vulnerability].GetValue() / 100.0f;
            finalDamage *= 1.0f - stats[StatNames.Toughness].GetValue() / 100.0f;
                
            damageResidue += finalDamage - Mathf.Floor(finalDamage);
            finalDamage = Mathf.Floor(finalDamage);
            if (damageResidue > 1.0f)
            {
                finalDamage += Mathf.Floor(damageResidue);
                damageResidue -= Mathf.Floor(damageResidue);
            }
            return finalDamage;
        }
        
        private void TakeDamage(float baseDamage, float finalDamage, Color damageColor)
        {
            if (baseDamage <= 0) return;
            
            health -= finalDamage;
            ballAnimator.HitFlash();
            audioHandler.PlayDamageSound();
            ScoreFloaterSpawner.Instance.SpawnScoreFloater(transform.position, finalDamage, damageColor);
            
            if (health <= 0) Die();
        }

        public void Heal(float amount)
        {
            float maxHealPossible = GetStat(StatNames.MaxHealth).GetValue() - health;
            float finalHealAmount = Mathf.Min(amount, maxHealPossible);
            health += finalHealAmount;
            
            if (finalHealAmount <= 0) return;
            ballAnimator.HitFlash(Color.limeGreen);
            ScoreFloaterSpawner.Instance.SpawnScoreFloater(transform.position, finalHealAmount, Color.limeGreen, "+");
        }

        private void DebugAttack(AttackData attackData)
        {
            Vector2 end = (Vector2)transform.position + attackData.knockback/5.0f;
            Debug.DrawLine(transform.position, end, attackData.source.GetColor(), 2);
        }

        public void Die()
        {
            if (debugLogToConsole) Debug.Log($"{name} died! :(");
            OnDeath?.Invoke();
            Destroy(gameObject, 0f);
        }

        private void OnWallBounce(BallCollisionData data)
        {
            audioHandler.PlayBounceSound();
            if (debugLogToConsole) Debug.Log($"{name} bounced!");
        }

        private void OnBallCollision(BallCollisionData data)
        {
            BattleBall otherBall = data.otherBall?.GetComponent<BattleBall>();
            if (!otherBall) return;
            
            AttackData attackData = new AttackData();
            attackData.source = this;
            attackData.damage = stats[StatNames.ContactDamage].GetValue();
            
            otherBall.ReceiveAttack(attackData);
            
            if (debugLogToConsole) Debug.Log($"{name} collided with {otherBall.name}, dealing {attackData.damage} damage!");
        }

        private void OnActiveAbilityUsed()
        {
            OnPrimaryAbilityUsed?.Invoke();
        }
        
        public PlayerInputRelaySO GetInputRelay() => inputRelay;
        public void SetInputRelay(PlayerInputRelaySO inputRelay) => this.inputRelay = inputRelay;
        public BallDataSO GetBallData() => ballData;
        public Sprite GetSprite() => ballData.ballSprite;
        public Color GetColor() => ballData.debugColor;
        public TeamNames GetTeam() => team;
        public float GetHealth() => health;
        public float GetRelativeHealth() => health / GetStat(StatNames.MaxHealth).GetValue();
        public void  ResetVelocity() => ballBody.SetVelocity(Vector2.zero);
        public Stat GetStat(StatNames stat) => stats[stat];
    }
}
