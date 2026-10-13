using UnityEngine;

namespace Balltallion.Simulation.Battle
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
        private const string playerWeaponLayer = "PlayerWeapon";
        private const string enemyWeaponLayer = "EnemyWeapon";

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

        public static int GetWeaponTeamLayerID(TeamNames team)
        {
            return LayerMask.NameToLayer(team == TeamNames.Player ? playerWeaponLayer : enemyWeaponLayer);
        }

        public static LayerMask GetTeamLayerMask(TeamNames team)
        {
            return 1 << GetTeamLayerID(team);
        }

        public static LayerMask GetProjectileTeamLayerMask(TeamNames team)
        {
            return 1 << GetProjectileTeamLayerID(team);
        }

        public static LayerMask GetWeaponTeamLayerMask(TeamNames team)
        {
            return 1 << GetWeaponTeamLayerID(team);
        }
    }
}