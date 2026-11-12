using UnityEngine;

namespace Balltallion.Simulation.Battle
{
    public struct AttackData
    {
        public BattleBall source;
        public float damage;
        public Vector2 knockback;
    }
}