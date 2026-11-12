using System.Collections.Generic;

namespace Balltallion.Simulation.Teams
{
    public class TeamLoadout
    {
        List<BallLoadout> ballLoadouts = new();

        public int currentBallIndex = -1;
        public void AddBall(BallLoadout ball) => ballLoadouts.Add(ball);
        public bool HasNextBall() => currentBallIndex < ballLoadouts.Count - 1;
        public BallLoadout GetNextBall() => ballLoadouts[++currentBallIndex];
        public int TeamSize() => ballLoadouts.Count;
        public void ResetIdx() => currentBallIndex = -1;
    }
}