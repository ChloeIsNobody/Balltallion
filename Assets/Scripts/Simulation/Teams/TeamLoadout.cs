using System.Collections.Generic;

namespace Balltallion.Simulation.Teams
{
    public class TeamLoadout
    {
        List<BallLoadout> ballLoadouts;

        public int currentBallIndex = -1;

        public TeamLoadout(List<BallDataSO> ballData)
        {
            ballLoadouts = ballData.ConvertAll(ball => new BallLoadout(ball));
        }
        
        public bool HasNextBall() => currentBallIndex < ballLoadouts.Count - 1;
        public BallLoadout GetNextBall() => ballLoadouts[++currentBallIndex];
        public int TeamSize() => ballLoadouts.Count;
        public void ResetIdx() => currentBallIndex = -1;
    }
}