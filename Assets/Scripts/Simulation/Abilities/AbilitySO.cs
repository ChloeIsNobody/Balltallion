using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    [CreateAssetMenu(menuName = "Balltallion/Ability", fileName = "New Ability")]
    public class AbilitySO : ScriptableObject
    {
        [SerializeReference, SubclassSelector] public AbilityTrigger trigger;
        [SerializeReference, SubclassSelector] public List<AbilityEffect> effects;
        public AbilityTrigger GetTriggerInstance() => trigger.Clone();
        public List<AbilityEffect> GetEffectInstances() => effects.Select(effect => effect.Clone()).ToList();
        
    }
}
