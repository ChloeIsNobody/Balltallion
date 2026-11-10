using System.Collections.Generic;
using Balltallion.Input;
using Balltallion.Simulation.Teams;
using Balltallion.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Balltallion.DebugTools
{
    public class NavigationDebugger : MonoBehaviour
    {
        [SerializeField] private LocalPlayerSO player1;
        [SerializeField] private LocalPlayerSO player2;
        [SerializeField] private List<BallDataSO> ballData;

        [SerializeField] private Transform player1Grid;
        [SerializeField] private Transform player2Grid;
        [SerializeField] private Transform sharedButtons;
        [SerializeField] private NavigationPointer navigationPointer;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private Button defaultButton;
        
        private LocalPlayerSO currentPlayer;

        private void Start()
        {
            SetCurrentPlayerTurn(player1);

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

        private void OnPlayer1ButtonClicked()
        {
            Debug.Log("Player 1 button clicked!");
            SwitchPlayerTurn();
        }

        private void OnPlayer2ButtonClicked()
        {
            Debug.Log("Player 2 button clicked!");
            SwitchPlayerTurn();
        }

        private void OnSharedButtonClicked()
        {
            Debug.Log("Shared button clicked!");
            SwitchPlayerTurn();
        }

        private void SwitchPlayerTurn()
        {
            SetCurrentPlayerTurn(currentPlayer == player1 ? player2 : player1);
        }

        private void SetCurrentPlayerTurn(LocalPlayerSO player)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
            currentPlayer = player;
            navigationPointer.SetColor(player.GetTeamColor());
            background.material = player.GetBackgroundMaterial();
        }
    }
}