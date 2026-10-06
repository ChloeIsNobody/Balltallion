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
        private const string playerTeamLayer = "PlayerBall";
        private const string enemyTeamLayer = "EnemyBall";

        public static int GetTeamLayer(TeamNames team)
        {
            return LayerMask.NameToLayer(team == TeamNames.Player ? playerTeamLayer : enemyTeamLayer);
        }
    }
}