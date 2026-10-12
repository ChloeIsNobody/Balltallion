using UnityEngine;

namespace Balltallion.Battle
{
    public enum TeamNames
    {
        Player,
        Enemy,
    }
    
    public static class TeamUtilities
    {
        private const string playerLayer = "PlayerBall";
        private const string enemyLayer = "EnemyBall";
        private const string playerProjectileLayer = "PlayerProjectile";
        private const string enemyProjectileLayer = "EnemyProjectile";

        public static int GetTeamLayer(TeamNames team)
        {
            return LayerMask.NameToLayer(team == TeamNames.Player ? playerLayer : enemyLayer);
        }

        public static int GetProjectileTeamLayer(TeamNames team)
        {
            return LayerMask.NameToLayer(team == TeamNames.Player ? playerProjectileLayer : enemyProjectileLayer);
        }
    }
}