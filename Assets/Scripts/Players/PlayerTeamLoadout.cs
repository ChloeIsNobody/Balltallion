using System;
using System.Collections.Generic;
using System.Linq;
using Balltallion.Simulation.Teams;
using UnityEngine;

namespace Balltallion.Players
{
    [Serializable]
    public class PlayerTeamLoadout
    {
        public event Action OnLoadoutUpdated;
        
        [SerializeField] private List<BallLoadout> ballLoadouts;

        private const int ballLimit = 3;
        private int loadoutIdx = -1;

        public PlayerTeamLoadout()
        {
            ballLoadouts = new List<BallLoadout>(ballLimit);
            for (int i = 0; i < ballLimit; i++) ballLoadouts.Add(new BallLoadout());
        }
        
        public void DraftBall(BallDataSO ball, int inventoryIdx)
        {
            BallLoadout currentLoadout = GetCurrentBallLoadout();
            currentLoadout?.SetBall(ball, inventoryIdx);
            OnLoadoutUpdated?.Invoke();
        }
        
        public void RemoveBall(int inventoryIdx)
        {
            BallLoadout loadout = GetBallFromInventoryIdx(inventoryIdx);
            BallDataSO removedBall = loadout?.GetBallData();
            loadout?.Clear();
            if (removedBall != null) OnLoadoutUpdated?.Invoke();
        }
        
        public void Clear()
        {
            foreach (BallLoadout loadout in ballLoadouts) loadout.Clear();
        }
        
        public BallLoadout GetBallLoadout(int idx) => ballLoadouts[idx];
        public BallLoadout GetBallFromInventoryIdx(int inventoryIdx) => ballLoadouts.FirstOrDefault(loadout => loadout.GetInventoryIdx() == inventoryIdx);
        public bool IsBallEquipped(int inventoryIdx) => ballLoadouts.Any(loadout => loadout.GetInventoryIdx() == inventoryIdx);
        public BallLoadout GetCurrentBallLoadout() => ballLoadouts[loadoutIdx];
        public BallLoadout GetFirstOpenLoadout() => ballLoadouts.FirstOrDefault(loadout => !loadout.GetBallData());
        
        public BallLoadout GetNextBall() => ballLoadouts[loadoutIdx++];
        public bool HasNextBall() => loadoutIdx < ballLoadouts.Count - 1;
        public void ResetLoadoutPointer() => loadoutIdx = 0;
        public int GetCurrentLoadoutIdx() => loadoutIdx;
    }
}