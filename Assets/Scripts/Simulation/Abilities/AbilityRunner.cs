using System.Collections.Generic;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    public class AbilityRunner : MonoBehaviour
    {
        private AbilityTrigger trigger;
        private List<AbilityEffect> effects;

        public void Initialize(AbilityTrigger triggerInstance, List<AbilityEffect> effectInstances)
        {
            trigger = triggerInstance;
            effects = effectInstances;
        }
    }
}