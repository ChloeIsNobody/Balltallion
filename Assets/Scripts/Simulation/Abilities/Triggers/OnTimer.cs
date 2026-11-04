using System;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnTimer : AbilityTrigger
    {
        [SerializeField] private float cooldown;
    }
}