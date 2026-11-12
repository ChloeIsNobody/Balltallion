namespace Balltallion.Simulation.Battle
{
    public interface IBattleManager
    {
        public BattleBall GetCurrentPlayerBall();
        public BattleBall GetCurrentEnemyBall();
        public void RestartBattle();
    }
}