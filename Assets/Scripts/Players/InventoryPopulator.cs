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
        
        [Header("Inventory")]
        [SerializeField] private List<BallDataSO> balls;
        [SerializeField] private List<EquipableDataSO> equipables;

        [Header("Loadouts")]
        [SerializeField] private bool populateLoadouts;
        [SerializeField] private List<int> loadoutBallIds;
        
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
            if (populateLoadouts) PopulateLoadout(localPlayer1.teamLoadout);
        }

        private void OnPlayer2Connected()
        {
            PopulateInventory(localPlayer2.inventory);
            if (populateLoadouts) PopulateLoadout(localPlayer2.teamLoadout);
        }

        private void PopulateInventory(PlayerInventory inventory)
        {
            inventory.Clear();
            foreach (BallDataSO ball in balls) inventory.AddBall(ball);
            foreach (EquipableDataSO equipable in equipables) inventory.AddEquipable(equipable);
        }

        private void PopulateLoadout(PlayerTeamLoadout loadout)
        {
            for (int i = 0; i < loadoutBallIds.Count; i++)
            {
                BallLoadout ballLoadout = loadout.GetBallLoadout(i);
                BallDataSO ballData = balls[loadoutBallIds[i]];
                ballLoadout.SetBall(ballData);
            }
        }
    }
}