namespace Balltallion.Simulation.Teams
{
    public class BallLoadout
    {
        private BallDataSO ball;
        // private List<EquipableDataSO> equipables;

        public BallLoadout(BallDataSO ballData)
        {
            ball = ballData;
        }
        
        public BallDataSO GetBallData() => ball;
    }
}