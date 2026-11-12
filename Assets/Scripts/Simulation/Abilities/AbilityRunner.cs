using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    public class AbilityRunner : MonoBehaviour
    {
        BattleBall battleBall;
        private AbilityTrigger trigger;
        private List<AbilityEffect> effects;

        public void Initialize(BattleBall battleBall, AbilityTrigger triggerInstance, List<AbilityEffect> effectInstances)
        {
            this.battleBall = battleBall;
            
            effects = effectInstances;
            foreach (AbilityEffect effect in effects) effect.BindToBall(battleBall);
            
            trigger = triggerInstance;
            trigger.OnTrigger += OnTrigger;
            trigger.OnUntrigger += OnUntrigger;
            trigger.BindToBall(battleBall);
        }

        private void OnTrigger()
        {
            foreach (AbilityEffect effect in effects) effect.Activate();
        }

        private void OnUntrigger()
        {
            foreach (AbilityEffect effect in effects) effect.Deactivate();
        }

        private void Update()
        {
            trigger.Update();
            foreach (AbilityEffect effect in effects) effect.Update();
        }
    }
}