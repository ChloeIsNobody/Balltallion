using Balltallion.Players;
using Balltallion.UI.Components.Navigation;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class DraftingManager : MonoBehaviour
    {
        [SerializeField] private LocalPlayerSO player1;
        [SerializeField] private LocalPlayerSO player2;
        [SerializeField, Scene] private string nextSceneToLoad;
        
        [Header("References")]
        [SerializeField] private NavigationManager navigationManager;
        [SerializeField] private LockInPrompt lockInPrompt;
        
        [Space]
        [SerializeField] private PlayerInventoryUI playerInventoryUI1;
        [SerializeField] private PlayerInventoryUI playerInventoryUI2;

        private BallLoadout currentBallLoadout;
        
        private void Awake()
        {
            player1.teamLoadout.ResetLoadoutPointer();
            playerInventoryUI1.BindToPlayer(player1);
            Selectable defaultSelection = playerInventoryUI1.GetFirstBallSlot();
            navigationManager.SetDefaultSelectionForPlayer(player1, defaultSelection);
            
            player2.teamLoadout.ResetLoadoutPointer();
            playerInventoryUI2.BindToPlayer(player2);
            defaultSelection = playerInventoryUI2.GetFirstBallSlot();
            navigationManager.SetDefaultSelectionForPlayer(player2, defaultSelection);
        }

        private void OnEnable()
        {
            navigationManager.OnPlayerTurnStarted += OnPlayerTurnStarted;
            lockInPrompt.OnLockedIn += OnLockedIn;
        }

        private void OnDisable()
        {
            navigationManager.OnPlayerTurnStarted -= OnPlayerTurnStarted;
            lockInPrompt.OnLockedIn -= OnLockedIn;
        }
        
        private void Update()
        {
            lockInPrompt.SetActive(currentBallLoadout.GetBallData());
        }

        private void OnPlayerTurnStarted(LocalPlayerSO player)
        {
            currentBallLoadout = player.teamLoadout.GetCurrentBallLoadout();
        }

        private void OnLockedIn()
        {
            navigationManager.GetCurrentPlayer().teamLoadout.MoveLoadoutPointer();
            navigationManager.EndPlayerTurn();
        }
    }
}