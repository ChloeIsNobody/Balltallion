using System.Collections;
using System.Collections.Generic;
using Balltallion.Input;
using Balltallion.Simulation.Teams;
using Balltallion.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Balltallion.DebugTools
{
    public class NavigationDebugger : MonoBehaviour
    {
        [SerializeField] private List<LocalPlayerSO> localPlayers;
        [SerializeField] private List<BallDataSO> ballData;
        [SerializeField] private InputManager inputManager;
        
        [SerializeField] private Transform player1Grid;
        [SerializeField] private Transform player2Grid;
        [SerializeField] private Transform sharedButtons;
        [SerializeField] private NavigationPointer navigationPointer;
        [SerializeField] private SpriteRenderer background;
        
        [SerializeField] private Button defaultButtonPlayer1;
        [SerializeField] private Button defaultButtonPlayer2;
        
        private LocalPlayerSO currentPlayer;

        private void Awake()
        {
            foreach (Transform child in player1Grid)
            {
                BallDataSO randomBall = ballData[Random.Range(0, ballData.Count)];
                Button button = child.GetComponent<Button>();
                Image image = button.transform.GetChild(0).GetComponent<Image>();
                image.sprite = randomBall.ballSprite;
                button.onClick.AddListener(OnPlayer1ButtonClicked);
            }

            foreach (Transform child in player2Grid)
            {
                BallDataSO randomBall = ballData[Random.Range(0, ballData.Count)];
                Button button = child.GetComponent<Button>();
                Image image = button.transform.GetChild(0).GetComponent<Image>();
                image.sprite = randomBall.ballSprite;
                button.onClick.AddListener(OnPlayer2ButtonClicked);
            }

            foreach (Transform child in sharedButtons)
            {
                Button button = child.GetComponent<Button>();
                button.onClick.AddListener(OnSharedButtonClicked);
            }
        }
        
        private void Start() => StartCoroutine(DelayedStart());
        
        // Has to be done just because of input state setup being finicky
        private IEnumerator DelayedStart()
        {
            yield return null;
            InitializeInput();
            yield return null;
            SetCurrentPlayerTurn(localPlayers[0]);
        }

        private void InitializeInput()
        {
            inputManager.ActivateSplitKeyboard();
            foreach (LocalPlayerSO localPlayer in localPlayers)
            {
                localPlayer.GetInputRelay().SwitchActionMap(ActionMaps.Navigation);
            }
        }

        private void OnPlayer1ButtonClicked()
        {
            Debug.Log("Player 1 button clicked!");
            EndCurrentPlayerTurn();
        }

        private void OnPlayer2ButtonClicked()
        {
            Debug.Log("Player 2 button clicked!");
            EndCurrentPlayerTurn();
        }

        private void OnSharedButtonClicked()
        {
            Debug.Log("Shared button clicked!");
            EndCurrentPlayerTurn();
        }

        private void EndCurrentPlayerTurn()
        {
            SetCurrentPlayerTurn(currentPlayer == localPlayers[0] ? localPlayers[1] : localPlayers[0]);
        }

        private void SetCurrentPlayerTurn(LocalPlayerSO player)
        {
            currentPlayer = player;
            
            GameObject defaultButton = player == localPlayers[0] ? defaultButtonPlayer1.gameObject : defaultButtonPlayer2.gameObject;
            EventSystem.current.SetSelectedGameObject(defaultButton);
            SetActionsAsset(player.GetInputRelay().GetActionAsset());
            
            player1Grid.GetComponent<CanvasGroup>().interactable = player == localPlayers[0];
            player2Grid.GetComponent<CanvasGroup>().interactable = player == localPlayers[1];
            
            navigationPointer.SetColor(player.GetTeamColor());
            background.material = player.GetBackgroundMaterial();
        }

        private void SetActionsAsset(InputActionAsset newActionAsset)
        {
            InputSystemUIInputModule inputModule = EventSystem.current.currentInputModule as InputSystemUIInputModule;
            if (!inputModule) return;
            inputModule.UnassignActions();
            inputModule.actionsAsset = newActionAsset;
            inputModule.point = InputActionReference.Create(newActionAsset.FindAction("Navigation/Point"));
            inputModule.leftClick = InputActionReference.Create(newActionAsset.FindAction("Navigation/Click"));
            inputModule.middleClick = InputActionReference.Create(newActionAsset.FindAction("Navigation/MiddleClick"));
            inputModule.rightClick = InputActionReference.Create(newActionAsset.FindAction("Navigation/RightClick"));
            inputModule.scrollWheel = InputActionReference.Create(newActionAsset.FindAction("Navigation/ScrollWheel"));
            inputModule.move = InputActionReference.Create(newActionAsset.FindAction("Navigation/Navigate"));
            inputModule.submit = InputActionReference.Create(newActionAsset.FindAction("Navigation/Submit"));
            inputModule.cancel = InputActionReference.Create(newActionAsset.FindAction("Navigation/Cancel"));
            inputModule.trackedDevicePosition = null;
            inputModule.trackedDeviceOrientation = null;
        }
    }
}