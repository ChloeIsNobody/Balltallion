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

        public static TeamNames GetOpposingTeam(TeamNames team)
        {
            return team == TeamNames.Player ? TeamNames.Enemy : TeamNames.Player;
        }

        public static int GetTeamLayerID(TeamNames team)
        {
            return LayerMask.NameToLayer(team == TeamNames.Player ? playerLayer : enemyLayer);
        }

        public static int GetProjectileTeamLayerID(TeamNames team)
        {
            return LayerMask.NameToLayer(team == TeamNames.Player ? playerProjectileLayer : enemyProjectileLayer);
        }

        public static LayerMask GetTeamLayerMask(TeamNames team)
        {
            return 1 << GetTeamLayerID(team);
        }

        public static LayerMask GetProjectileTeamLayerMask(TeamNames team)
        {
            return 1 << GetProjectileTeamLayerID(team);
        }
    }
}