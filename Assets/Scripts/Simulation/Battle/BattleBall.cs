using System;
using System.Collections.Generic;
using Balltallion.Audio;
using Balltallion.Simulation.Feel;
using Balltallion.Simulation.Physics;
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
        
        [Header("Debug Draw")]
        [SerializeField] private bool debugLogToConsole = false;
        [SerializeField] private bool debugAttacks;
        
        private int health;
        private BallBody ballBody;

        private void Awake()
        {
            ballBody = GetComponent<BallBody>();
        }

        private void Start()
        {
            if (ballData) LoadBallData(ballData, team);
        }

        [Button("Editor Refresh Stats")]
        private void EditorLoadStats()
        {
            if (ballData != null) LoadBallData(ballData, team);
        }

        public void LoadBallData(BallDataSO data, TeamNames team)
        {
            ballData = data;
            health = data.maxHealth;
            
            SetTeam(team);
            name = data.displayName;
            ballAnimator.SetSprite(data.ballSprite);
            
            if (!ballBody) ballBody = GetComponent<BallBody>();
            ballBody.LoadBallData(data);
            ballAnimator.LoadBallData(data);
            
            weaponController.gameObject.SetActive(data.weapon != null);
            weaponController.LoadWeaponData(data.weapon);

            ClearChildAbilities();
            if (data.abilityPrefab)
            {
                GameObject ability = Instantiate(data.abilityPrefab, abilitiesParent.transform);
                ability.transform.localPosition = Vector3.zero;
            }

            audioHandler.LoadBallData(data);
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

        public void Attack(AttackData attackData)
        {
            TakeDamage(attackData.damage);
            ballBody.ApplyKnockback(attackData.knockback);
            if (debugAttacks) DebugAttack(attackData);
            
            if (attackData.damage > 0 )
            {
                Color color = attackData.source ? attackData.source.GetColor() : Color.white;
                ScoreFloaterSpawner.Instance.SpawnScoreFloater(transform.position, attackData.damage, color);
            }
        }

        private void DebugAttack(AttackData attackData)
        {
            Vector2 end = (Vector2)transform.position + attackData.knockback/5.0f;
            Debug.DrawLine(transform.position, end, attackData.source.GetColor(), 2);
        }

        private void TakeDamage(int damage)
        {
            if (damage <= 0) return;
            health -= damage;
            ballAnimator.HitFlash();
            audioHandler.PlayDamageSound();
            if (health <= 0) Die();
        }

        public void Die()
        {
            if (debugLogToConsole) Debug.Log($"{name} died! :(");
            OnDeath?.Invoke();
            Destroy(gameObject, 0f);
        }
        
        public float GetHealth() => health;
        public float GetMaxHealth() => ballData.maxHealth;

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
            
            if (ballData.velocityScaledContactDamage)
            {
                attackData.damage = ballData.GetVelocityScaledDamage(data.myCollisionPower);
            }
            else attackData.damage = ballData.contactDamage;
            
            otherBall.Attack(attackData);
            
            if (debugLogToConsole) Debug.Log($"{name} collided with {otherBall.name}, dealing {attackData.damage} damage!");
        }
        
        public BallDataSO GetBallData() => ballData;
        public Sprite GetSprite() => ballData.ballSprite;
        public Color GetColor() => ballData.debugColor;
        public TeamNames GetTeam() => team;
    }
}
