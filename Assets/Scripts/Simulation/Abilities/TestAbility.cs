using System.Collections;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    public class TestAbility : ActiveAbility
    {
        private void Start()
        {
            InitializeStateMachine(DummyState, nameof(DummyState));
        }
        
        private IEnumerator DummyState()
        {
            yield return new WaitForSeconds(1.0f);
            TransitionToCooldown();
        }
    }
}