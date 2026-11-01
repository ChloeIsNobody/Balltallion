using System;
using System.Collections.Generic;
using Balltallion.Audio;
using Balltallion.Input;
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
        
        [Header("Loaded Data")]
        [SerializeField, Expandable] private BallDataSO ballData;
        [SerializeField] private TeamNames team;
        
        [Header("References")]
        [SerializeField] private WeaponController weaponController;
        [SerializeField] private BallAnimator ballAnimator;
        [SerializeField] private BallAudioHandler audioHandler;
        [SerializeField] private Transform abilitiesParent;
        
        [Header("Debug")]
        [SerializeField] private bool debugLogToConsole = false;
        [SerializeField] private bool debugAttacks;
        
        private int health;
        private bool isInvulnerable;
        private BallBody ballBody;
        private PlayerInputRelaySO inputRelay;

        private Dictionary<StatNames, Stat> stats = new();
        
        public enum StatNames
        {
            MaxHealth,
            Size,
        }

        private void Awake()
        {
            ballBody = GetComponent<BallBody>();
        }
        
        private void OnEnable()
        {
            ballBody.OnWallBounce += OnWallBounce;
            ballBody.OnBallCollision += OnBallCollision;
        }

        private void OnDisable()
        {
            ballBody.OnWallBounce -= OnWallBounce;
            ballBody.OnBallCollision -= OnBallCollision;
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
            health = (int)stats[StatNames.MaxHealth].GetValue();
            
            SetTeam(team);
            name = data.displayName;
            ballAnimator.SetSprite(data.ballSprite);
            
            if (!ballBody) ballBody = GetComponent<BallBody>();
            ballBody.LoadBallData(data);
            ballBody.LoadSizeStat(stats[StatNames.Size]);
            
            ballAnimator.LoadBallData(data);
            
            weaponController.gameObject.SetActive(data.weapon);
            weaponController.LoadWeaponData(data.weapon);

            ClearChildAbilities();
            if (data.abilityPrefab)
            {
                GameObject ability = Instantiate(data.abilityPrefab, abilitiesParent.transform);
                ability.transform.localPosition = Vector3.zero;
            }

            audioHandler.LoadBallData(data);
        }
        
        private void InitializeStats()
        {
            stats = new Dictionary<StatNames, Stat>
            {
                [StatNames.MaxHealth] = new(nameof(StatNames.MaxHealth), ballData.GetBaseStat(StatNames.MaxHealth), "F0"),
                [StatNames.Size] = new(nameof(StatNames.Size), ballData.GetBaseStat(StatNames.Size), "F2", 0.1f, 3.0f),
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
            TakeDamage(attackData.damage, damageColor);
            ballBody.ApplyKnockback(attackData.knockback);
            if (debugAttacks) DebugAttack(attackData);
        }
        private void TakeDamage(int damage, Color damageColor)
        {
            if (isInvulnerable) damage = 0;
            if (damage <= 0) return;
            
            health -= damage;
            ballAnimator.HitFlash();
            audioHandler.PlayDamageSound();
            ScoreFloaterSpawner.Instance.SpawnScoreFloater(transform.position, damage, damageColor);
            
            if (health <= 0) Die();
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
            attackData.damage = ballData.contactDamage;
            
            otherBall.ReceiveAttack(attackData);
            
            if (debugLogToConsole) Debug.Log($"{name} collided with {otherBall.name}, dealing {attackData.damage} damage!");
        }
        
        public PlayerInputRelaySO GetInputRelay() => inputRelay;
        public void SetInputRelay(PlayerInputRelaySO inputRelay) => this.inputRelay = inputRelay;
        public BallDataSO GetBallData() => ballData;
        public Sprite GetSprite() => ballData.ballSprite;
        public Color GetColor() => ballData.debugColor;
        public TeamNames GetTeam() => team;
        public float GetHealth() => health;
        public Stat GetStat(StatNames stat) => stats[stat];
        public bool IsInvulnerable() => isInvulnerable;
        public void SetInvulnerable(bool invulnerable) => isInvulnerable = invulnerable;
    }
}
