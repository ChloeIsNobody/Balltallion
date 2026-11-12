using System.Collections.Generic;
using Balltallion.Simulation.Abilities;

namespace Balltallion.Simulation.Teams
{
    public class BallLoadout
    {
        private BallDataSO ball;
        private List<EquipableDataSO> equipables;

        public BallLoadout(BallDataSO ballData, List<EquipableDataSO> equipables)
        {
            ball = ballData;
            this.equipables = equipables;
            if (equipables == null) this.equipables = new List<EquipableDataSO>();
        }
        
        public BallDataSO GetBallData() => ball;
        public IReadOnlyList<EquipableDataSO> GetEquipables() => equipables;
    }
}