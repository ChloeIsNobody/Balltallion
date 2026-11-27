using Balltallion.Players;
using Balltallion.Simulation.Abilities;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Teams;
using Balltallion.UI.Components.PagedGrid;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class DraftingManager : MonoBehaviour
    {
        [SerializeField, Scene] private string nextSceneToLoad;
        [SerializeField] private Button nextSceneButton;

        [SerializeField] private BallInventorySlot ballInventorySlotPrefab;
        [SerializeField] private EquipableInventorySlot equipableInventorySlotPrefab;

        [Header("Player 1")]
        [SerializeField] private LocalPlayerSO player1;
        [SerializeField] private PagedGridLayout ballGrid1;
        [SerializeField] private PagedGridLayout equipableGrid1;
        
        [Header("Player 2")]
        [SerializeField] private LocalPlayerSO player2;
        [SerializeField] private PagedGridLayout ballGrid2;
        [SerializeField] private PagedGridLayout equipableGrid2;

        private void Awake()
        {
            LoadInventory(player1, ballGrid1, equipableGrid1);
            LoadInventory(player2, ballGrid2, equipableGrid2);
        }

        private void LoadInventory(LocalPlayerSO player, PagedGridLayout ballGrid, PagedGridLayout equipableGrid)
        {
            PlayerInventory inventory = player.inventory;
            
            ballGrid.ClearChildren();
            for (int i = 0; i < inventory.GetBallCapacity(); i++)
            {
                int cellIdx = RemapCellIdx(i, ballGrid);
                BallDataSO ball = inventory.GetBallAtIdx(cellIdx);
                BallInventorySlot newSlot = Instantiate(ballInventorySlotPrefab, ballGrid.transform);
                newSlot.LoadData(ball, player);
            }
            
            equipableGrid.ClearChildren();
            for (int i = 0; i < inventory.GetEquipableCapacity(); i++)
            {
                int cellIdx = RemapCellIdx(i, equipableGrid);
                EquipableDataSO equipable = inventory.GetEquipableAtIdx(cellIdx);
                EquipableInventorySlot newSlot = Instantiate(equipableInventorySlotPrefab, equipableGrid.transform);
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

        private void OnEnable()
        {
            nextSceneButton.onClick.AddListener(GoToNextScene);
        }

        private void OnDisable()
        {
            nextSceneButton.onClick.RemoveAllListeners();
        }

        private void GoToNextScene()
        {
            SceneManager.LoadScene(nextSceneToLoad);
        }
    }
}