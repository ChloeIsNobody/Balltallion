using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    public class AbilityBuilder : MonoBehaviour
    {
        [SerializeField] private BattleBall battleBall;
        [SerializeField] private List<AbilitySO>  abilities = new List<AbilitySO>();
        [SerializeField] private AbilityRunner abilityRunnerPrefab;
        
        private List<AbilityRunner> abilityRunners = new List<AbilityRunner>();

        private void Start()
        {
            BuildAbilities(abilities);
        }

        private void BuildAbilities(List<AbilitySO> abilities)
        {
            foreach (AbilitySO ability in abilities)
            {
                AbilityRunner abilityRunner = Instantiate(abilityRunnerPrefab, transform);
                abilityRunner.name = $"AbilityRunner_{ability.name}";
                
                AbilityTrigger triggerInstance = ability.GetTriggerInstance();
                List<AbilityEffect> effectInstances = ability.GetEffectInstances();
                abilityRunner.Initialize(triggerInstance, effectInstances);
            }
        }
    }
}
