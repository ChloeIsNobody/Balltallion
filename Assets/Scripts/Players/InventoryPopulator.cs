using System.Collections.Generic;
using Balltallion.Simulation.Abilities;
using Balltallion.Simulation.Teams;
using UnityEngine;

namespace Balltallion.Players
{
    public class InventoryPopulator : MonoBehaviour
    {
        [SerializeField] private LocalPlayerSO localPlayer1;
        [SerializeField] private LocalPlayerSO localPlayer2;
        
        [SerializeField] private List<BallDataSO> balls;
        [SerializeField] private List<EquipableDataSO> equipables;
        
        private void Awake()
        {
            localPlayer1.OnConnected += OnPlayer1Connected;
            localPlayer2.OnConnected += OnPlayer2Connected;
        }

        private void OnDestroy()
        {
            localPlayer1.OnConnected -= OnPlayer1Connected;
            localPlayer2.OnConnected -= OnPlayer2Connected;
        }
        
        private void OnPlayer1Connected()
        {
            PopulateInventory(localPlayer1.inventory);
        }

        private void OnPlayer2Connected()
        {
            PopulateInventory(localPlayer2.inventory);
        }

        private void PopulateInventory(PlayerInventory inventory)
        {
            inventory.Clear();
            foreach (BallDataSO ball in balls) inventory.AddBall(ball);
            foreach (EquipableDataSO equipable in equipables) inventory.AddEquipable(equipable);
        }
    }
}