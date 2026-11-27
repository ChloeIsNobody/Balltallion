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
            LoadInventory(player1.inventory, ballGrid1, equipableGrid1);
            LoadInventory(player2.inventory, ballGrid2, equipableGrid2);
        }

        private void LoadInventory(PlayerInventory inventory, PagedGridLayout ballGrid, PagedGridLayout equipableGrid)
        {
            ballGrid.ClearChildren();
            int ballPageCount = ballGrid.GetNumPagesToHoldCells(inventory.GetBallCount());
            int ballSlotCount = ballPageCount * ballGrid.GetCellsPerPage();
            for (int i = 0; i < ballSlotCount; i++)
            {
                BallDataSO ball = inventory.GetBallAtIdx(i);
                InstantiateBallInventorySlot(ballGrid.transform, ball);
            }
            
            equipableGrid.ClearChildren();
            int equipablePageCount = equipableGrid.GetNumPagesToHoldCells(inventory.GetEquipableCount());
            int equipableSlotCount = equipablePageCount * equipableGrid.GetCellsPerPage();
            for (int i = 0; i < equipableSlotCount; i++)
            {
                EquipableDataSO equipable = inventory.GetEquipableAtIdx(i);
                InstantiateEquipableInventorySlot(equipableGrid.transform, equipable);
            }
        }

        private void InstantiateBallInventorySlot(Transform parent, BallDataSO ballData)
        {
            BallInventorySlot newSlot = Instantiate(ballInventorySlotPrefab, parent);
            newSlot.LoadData(ballData);
        }

        private void InstantiateEquipableInventorySlot(Transform parent, EquipableDataSO equipableData)
        {
            EquipableInventorySlot newSlot = Instantiate(equipableInventorySlotPrefab, parent);
            newSlot.LoadData(equipableData);
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