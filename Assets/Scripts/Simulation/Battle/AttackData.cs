using UnityEngine;

namespace Balltallion.Simulation.Battle
{
    public struct AttackData
    {
        public BattleBall source;
        public int damage;
        public Vector2 knockback;
    }
}