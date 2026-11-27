using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Balltallion.Players
{
    [Serializable]
    public class PlayerTeamLoadout
    {
        [SerializeField] private List<BallLoadout> ballLoadouts;

        private const int ballLimit = 3;
        private int currentBallIndex = -1;

        public PlayerTeamLoadout()
        {
            ballLoadouts = new List<BallLoadout>(ballLimit);
            for (int i = 0; i < ballLimit; i++) ballLoadouts.Add(new BallLoadout());
        }

        public void Clear()
        {
            foreach (BallLoadout loadout in ballLoadouts) loadout.Clear();
        }

        public BallLoadout GetFirstOpenLoadout() => ballLoadouts.FirstOrDefault(loadout => !loadout.GetBallData());
        public BallLoadout GetBallLoadout(int idx) => ballLoadouts[idx];
        
        public int TeamSize() => ballLoadouts.Count;
        public bool HasNextBall() => currentBallIndex < ballLoadouts.Count - 1;
        public BallLoadout GetNextBall() => ballLoadouts[++currentBallIndex];
        public void ResetIdx() => currentBallIndex = -1;
    }
}