using Balltallion.Simulation.Battle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.Simulation.Abilities
{
    public class KnightAbility : MonoBehaviour
    {
        private BattleBall parentBall;

        private void Awake()
        {
            parentBall = GetComponentInParent<BattleBall>();
        }
        
        private void Update()
        {
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                Debug.Log($"Ability activated on {parentBall.name}");
            }
        }
    }
}