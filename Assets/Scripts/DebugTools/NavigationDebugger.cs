using System.Collections;
using System.Collections.Generic;
using Balltallion.Input;
using Balltallion.Simulation.Abilities;
using Balltallion.Simulation.Teams;
using Balltallion.UI;
using Balltallion.UI.Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.DebugTools
{
    public class NavigationDebugger : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private NavigationManager navigationManager;
        [SerializeField] private TabMenuSelector inventoryTabSelector1;
        [SerializeField] private TabMenuSelector inventoryTabSelector2;
        
        [SerializeField] private LocalPlayerSO firstPlayer;
        [SerializeField] private CanvasGroup devicePairingScreen;
        [SerializeField] private CanvasGroup navigationScreen;
        [SerializeField] private SpriteRenderer background;

        [Header("Inventory Population")]
        [SerializeField] private List<BallDataSO> ballData;
        [SerializeField] private List<Transform> ballInventories;
        [SerializeField] private List<EquipableDataSO> equipableData;
        [SerializeField] private List<Transform> equipableInventories;
        [SerializeField] private Transform sharedButtons;

        private void Awake()
        {
            PopulateInventories();
        }

        private void PopulateInventories()
        {
            List<Button> ballButtons = new List<Button>();
            foreach (Transform inventory in ballInventories) ballButtons.AddRange(inventory.GetComponentsInChildren<Button>());
            List<Button> equipableButtons = new List<Button>();
            foreach (Transform inventory in equipableInventories) equipableButtons.AddRange(inventory.GetComponentsInChildren<Button>());
            
            foreach (Button button in ballButtons)
            {
                BallDataSO randomBall = ballData[Random.Range(0, ballData.Count)];
                Image image = button.transform.GetChild(0).GetComponent<Image>();
                image.sprite = randomBall.ballSprite;
                button.onClick.AddListener(OnButtonClicked);
            }

            foreach (Button button in equipableButtons)
            {
                EquipableDataSO randomEquipable = equipableData[Random.Range(0, equipableData.Count)];
                Image image = button.transform.GetChild(0).GetComponent<Image>();
                image.sprite = randomEquipable.icon;
                button.onClick.AddListener(OnButtonClicked);
            }

            foreach (Transform child in sharedButtons)
            {
                Button button = child.GetComponent<Button>();
                button.onClick.AddListener(OnButtonClicked);
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
            inventoryTabSelector1.OnTabSelected += OnTabSelected;
            inventoryTabSelector2.OnTabSelected += OnTabSelected;
        }

        private void OnDisable()
        {
            navigationManager.OnPlayerTurnStarted -= OnPlayerTurnStarted;
            inventoryTabSelector1.OnTabSelected -= OnTabSelected;
            inventoryTabSelector2.OnTabSelected -= OnTabSelected;
        }

        private void OnButtonClicked()
        {
            navigationManager.EndPlayerTurn();
        }

        private void OnTabSelected()
        {
            LocalPlayerSO player = navigationManager.GetCurrentPlayer();
            TabMenuSelector currentTabMenu = player.GetPlayerID() == 1 ? inventoryTabSelector1 : inventoryTabSelector2;
            CanvasGroup currentMenu = currentTabMenu.GetSelectedCanvasGroup();
            Selectable firstOption = currentMenu.transform.GetChild(0).GetComponent<Selectable>();
            firstOption.Select();
        }

        private void OnPlayerTurnStarted(LocalPlayerSO player)
        {
            TabMenuSelector selector = player.GetPlayerID() == 1 ? inventoryTabSelector1 : inventoryTabSelector2;
            selector.SelectTabIdx(0);
            background.material = player.GetBackgroundMaterial();
        }
    }
}