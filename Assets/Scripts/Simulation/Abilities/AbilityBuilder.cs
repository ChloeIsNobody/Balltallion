using System.Collections.Generic;
using System.Linq;
using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    [DisallowMultipleComponent]
    public class AbilityBuilder : MonoBehaviour
    {
        private BattleBall owner;
        private List<AbilitySO> abilities = new();

        public BattleBall Owner => owner;

        public IReadOnlyList<AbilitySO> Abilities => abilities;

        public void BuildAbilities(BattleBall ball, IReadOnlyList<AbilitySO> abilities)
        {
            this.Stop();
            if (!Application.isPlaying) return;

            owner = ball;
            if (abilities == null) return;

            foreach (AbilitySO ability in abilities)
            {
                if (!ability) continue;
                AbilitySO abilityCopy = ability.CreateRuntimeCopy();
                abilities.Append(abilityCopy);
                abilityCopy.Grant(this);
            }
        }

        public void Stop()
        {
            StopAllCoroutines();

            foreach (AbilitySO ability in abilities)
            {
                if (ability is null) continue;
                ability.Revoke();
                if (ability) Destroy(ability);
            }
            abilities.Clear();
            owner = null;
        }

        private void OnEnable()
        {
            if (!Application.isPlaying) return;
            foreach (AbilitySO ability in abilities)
                if (ability && !ability.IsBound) ability.Grant(this);
        }

        private void OnDisable()
        {
            foreach (AbilitySO ability in abilities)
            {
                if (ability is not null) ability.Revoke();
            }
        }

        private void OnDestroy()
        {
            this.Stop();
        }
    }
}
