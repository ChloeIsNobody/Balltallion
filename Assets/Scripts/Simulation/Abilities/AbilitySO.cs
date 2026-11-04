using System.Collections.Generic;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    [CreateAssetMenu(menuName = "Balltallion/Ability", fileName = "New Ability")]
    public class AbilitySO : ScriptableObject
    {
        [SerializeReference, SubclassSelector] AbilityTrigger trigger;
        [SerializeReference, SubclassSelector] List<AbilityEffect> effects;
        
        
    }
}
