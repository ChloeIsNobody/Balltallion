using System;
using System.Collections.Generic;
using Balltallion.Simulation.Abilities;
using Balltallion.Simulation.Teams;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Players
{
    [Serializable]
    public class PlayerInventory
    {
        [SerializeField] private int ballCapacity;
        [SerializeField] private int equipableCapacity;

        [SerializeField] private List<BallDataSO> balls = new();
        [SerializeField] private List<EquipableDataSO> equipables = new();

        public PlayerInventory(int ballCapacity, int equipableCapacity)
        {
            this.ballCapacity = ballCapacity;
            this.equipableCapacity = equipableCapacity;
        }

        public void AddBall(BallDataSO ball) => balls.Add(ball);
        public void AddEquipable(EquipableDataSO equipable) => equipables.Add(equipable);
        
        public void RemoveBall(BallDataSO ball) => balls.Remove(ball);
        public void RemoveEquipable(EquipableDataSO equipable) => equipables.Remove(equipable);
        
        public int GetBallCount() => balls.Count;
        public int GetEquipableCount() => equipables.Count;
        
        public int GetBallCapacity() => ballCapacity;
        public int GetEquipableCapacity() => equipableCapacity;
        
        public bool CanAddBall(BallDataSO ball) => balls.Count < ballCapacity;
        public bool CanAddEquipable(EquipableDataSO equipable) => equipables.Count < equipableCapacity;
        
        public BallDataSO GetBallAtIdx(int idx) => idx > balls.Count - 1 ? null : balls[idx];
        public EquipableDataSO GetEquipableAtIdx(int idx) => idx > equipables.Count - 1 ? null : equipables[idx];
        

        public void Clear()
        {
            balls.Clear();
            equipables.Clear();
        }
    }
}