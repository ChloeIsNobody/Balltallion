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
        [SerializeField] private InputManager inputManager;
        [SerializeField] private NavigationManager navigationManager;
        [SerializeField] private List<BallDataSO> ballData;
        [SerializeField] private LocalPlayerSO firstPlayer;

        [SerializeField] private CanvasGroup devicePairingScreen;
        [SerializeField] private CanvasGroup navigationScreen;
        
        [SerializeField] private Transform player1Grid;
        [SerializeField] private Transform player2Grid;
        [SerializeField] private Transform sharedButtons;
        [SerializeField] private SpriteRenderer background;

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

        private void Start()
        {
            
            navigationManager.SetFirstPlayer(firstPlayer);
            StartCoroutine(DevicePairingState());
        }
        
        private IEnumerator DevicePairingState()
        {
            CanvasGroupUtilities.SetCanvasGroupEnabled(devicePairingScreen, true);
            CanvasGroupUtilities.SetCanvasGroupEnabled(navigationScreen, false);
            inputManager.StartDevicePairing();
            
            yield return new WaitUntil(inputManager.AreBothPlayersReady);
            
            inputManager.EndDevicePairing();
            CanvasGroupUtilities.SetCanvasGroupEnabled(devicePairingScreen, false);
            CanvasGroupUtilities.SetCanvasGroupEnabled(navigationScreen, true);
            inputManager.SetPlayerActionMaps(ActionMaps.Navigation);
        }

        private void OnEnable()
        {
            navigationManager.OnPlayerTurnStarted += OnPlayerTurnStarted;
        }

        private void OnDisable()
        {
            navigationManager.OnPlayerTurnStarted -= OnPlayerTurnStarted;
        }
        
        private void OnPlayer1ButtonClicked()
        {
            Debug.Log("Player 1 button clicked!");
            navigationManager.EndPlayerTurn();
        }

        private void OnPlayer2ButtonClicked()
        {
            Debug.Log("Player 2 button clicked!");
            navigationManager.EndPlayerTurn();
        }

        private void OnSharedButtonClicked()
        {
            Debug.Log("Shared button clicked!");
            navigationManager.EndPlayerTurn();
        }

        private void OnPlayerTurnStarted(LocalPlayerSO player)
        {
            background.material = player.GetBackgroundMaterial();
        }
    }
}