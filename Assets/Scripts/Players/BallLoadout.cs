using System;
using System.Collections.Generic;
using Balltallion.Simulation.Abilities;
using Balltallion.Simulation.Teams;
using UnityEngine;

namespace Balltallion.Players
{
    [Serializable]
    public class BallLoadout
    {
        public event Action OnLoadoutUpdated;
        
        [SerializeField] private BallDataSO ball;
        [SerializeField] private List<EquipableDataSO> equipables;

        private const int equipableLimit = 3;

        public BallLoadout(BallDataSO ballData = null, List<EquipableDataSO> equipables = null)
        {
            ball = ballData;
            this.equipables = equipables;
            if (equipables == null) this.equipables = new List<EquipableDataSO>(equipableLimit);
        }

        public void Clear()
        { 
            for (int i = 0; i < equipables.Count; i++) equipables[i] = null;
            SetBall(null);
        }

        public void SetBall(BallDataSO newBall)
        {
            ball = newBall;
            OnLoadoutUpdated?.Invoke();
        }
        
        public BallDataSO GetBallData() => ball;
        public IReadOnlyList<EquipableDataSO> GetEquipables() => equipables;
    }
}