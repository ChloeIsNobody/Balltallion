using Balltallion.Players;
using Balltallion.Simulation.Abilities;
using Balltallion.Simulation.Teams;
using Balltallion.UI.Components.PagedGrid;
using UnityEngine;

namespace Balltallion.UI.Drafting
{
    public class PlayerInventoryUI : MonoBehaviour
    {
        [SerializeField] private LocalPlayerSO player;
        
        [Header("References")]
        [SerializeField] private PagedGridLayout ballGrid;
        [SerializeField] private PagedGridLayout equipableGrid;
        [SerializeField] private InventoryBallSlot inventoryBallSlotPrefab;
        [SerializeField] private InventoryEquipableSlot inventoryEquipableSlotPrefab;
        
        private void Awake()
        {
            LoadInventory();
        }

        private void LoadInventory()
        {
            PlayerInventory inventory = player.inventory;
            
            ballGrid.ClearChildren();
            for (int cellIdx = 0; cellIdx < inventory.GetBallCapacity(); cellIdx++)
            {
                int ballIdx = RemapCellIdx(cellIdx, ballGrid);
                InventoryBallSlot newSlot = Instantiate(inventoryBallSlotPrefab, ballGrid.transform);
                newSlot.BindToPlayer(player, ballIdx);
            }
            
            equipableGrid.ClearChildren();
            for (int i = 0; i < inventory.GetEquipableCapacity(); i++)
            {
                int cellIdx = RemapCellIdx(i, equipableGrid);
                EquipableDataSO equipable = inventory.GetEquipableAtIdx(cellIdx);
                InventoryEquipableSlot newSlot = Instantiate(inventoryEquipableSlotPrefab, equipableGrid.transform);
                newSlot.LoadData(equipable, player);
            }
        }
        
        /*
         * Remaps the cells so that rather than being placed column-wise, they are placed row-wise.
         *
         * This is necessary because the PagedGridLayout doesn't support row-wise layout.
         * Trying to achieve it directly through the grid layout settings would break paging.
         */
        private int RemapCellIdx(int cellIdx, PagedGridLayout grid)
        {
            int rows = grid.GetPageRows();
            int cols = grid.GetPageColumns();
            int pageSize = rows * cols;
            int pageIdx = cellIdx / pageSize;
            cellIdx %= pageSize;

            int rowIdx = cellIdx % rows;
            int colIdx = cellIdx / rows;
            int mappedCellIdx = rowIdx * cols + colIdx;

            return mappedCellIdx + pageIdx * pageSize;
        }
    }
}