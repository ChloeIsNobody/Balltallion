using System.Collections;
using Balltallion.Input;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Balltallion.UI.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField, Scene] private string loadSceneAfterPlay;
        
        [Header("Title Screen")]
        [SerializeField] private CanvasGroup titleScreen;
        [SerializeField] private Button playerButton;
        [SerializeField] private Button quitButton;
        
        [Header("Device Pairing Screen")]
        [SerializeField] private CanvasGroup pairingScreen;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private int readyCountdownDuration;
        
        private InputManager inputManager;
        private CanvasGroup currentScreen;
        private bool countdownCompleted;
        
        private void Awake()
        {
            CanvasGroupUtilities.SetCanvasGroupEnabled(titleScreen, false);
            CanvasGroupUtilities.SetCanvasGroupEnabled(pairingScreen, false);
            SwitchScreen(titleScreen);
        }

        private void Start()
        {
            inputManager = InputManager.Instance;
        }

        private void OnEnable()
        {
            playerButton.onClick.AddListener(PlayButtonClicked);
            quitButton.onClick.AddListener(QuitButtonClicked);
        }

        private void OnDisable()
        {
            playerButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
        }

        private void PlayButtonClicked()
        {
            StartCoroutine(DevicePairingState());
        }

        private void QuitButtonClicked()
        {
            Application.Quit();
        }
        
        private IEnumerator DevicePairingState()
        {
            SwitchScreen(pairingScreen);
            inputManager.StartDevicePairing();
            
            while (true)
            {
                yield return new WaitUntil(inputManager.AreBothPlayersReady);

                countdownCompleted = false;
                yield return StartCoroutine(CountdownCoroutine());
                if (countdownCompleted) break;
            }
            
            inputManager.EndDevicePairing();
            inputManager.SetPlayerActionMaps(ActionMaps.Navigation);
            SceneManager.LoadScene(loadSceneAfterPlay);
        }

        private IEnumerator CountdownCoroutine()
        {
            countdownText.enabled = true;

            bool countdownInterrupted = false;
            
            float countdownTimer = readyCountdownDuration;
            while (countdownTimer > 0)
            {
                countdownTimer -= Time.deltaTime;
                countdownText.text = $"{Mathf.CeilToInt(countdownTimer)}";
                if (!inputManager.AreBothPlayersReady())
                {
                    countdownInterrupted = true;
                    countdownText.enabled = false;
                    break;
                }
                yield return null;
            }
            
            countdownCompleted = !countdownInterrupted;
        }

        private void SwitchScreen(CanvasGroup newScreen)
        {
            if (currentScreen == newScreen) return;
            CanvasGroupUtilities.SetCanvasGroupEnabled(currentScreen, false);
            CanvasGroupUtilities.SetCanvasGroupEnabled(newScreen, true);
            currentScreen = newScreen;
        }
    }
}