using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Balltallion.Input
{
    public class DevicePairingScene : MonoBehaviour
    {
        [SerializeField] private List<LocalPlayerSO> localPlayers;
        [SerializeField] private GameObject splitKeyboardPrompt;
        [SerializeField] private TextMeshProUGUI countdownText;
        
        [SerializeField] private int readyCountdownDuration = 3;
        
        private InputManager inputManager;
        private Coroutine stateCoroutine;
        
        private void Start()
        {
            inputManager = InputManager.Instance;
            inputManager.StartDevicePairing();
            SetStateCoroutine(DevicePairingState());
        }

        private void Update()
        {
            splitKeyboardPrompt.gameObject.SetActive(inputManager.CanActivateSplitKeyboard);
        }

        private IEnumerator DevicePairingState()
        {
            countdownText.enabled = false;
            yield return new WaitUntil(AreBothPlayersReady);
            SetStateCoroutine(CountdownState());
        }

        private IEnumerator CountdownState()
        {
            countdownText.enabled = true;
            float countdownTimer = readyCountdownDuration;
            while (countdownTimer > 0)
            {
                countdownTimer -= Time.deltaTime;
                countdownText.text = $"{Mathf.CeilToInt(countdownTimer)}";
                if (!AreBothPlayersReady())
                {
                    SetStateCoroutine(DevicePairingState());
                    break;
                }
                yield return null;
            }
            
            countdownText.text = "Go!";
            yield return new WaitForSeconds(1);
            inputManager.EndDevicePairing();
            SceneManager.LoadScene("TeamPicker");
        }

        private void SetStateCoroutine(IEnumerator newState)
        {
            if (stateCoroutine != null) StopCoroutine(stateCoroutine);
            stateCoroutine = StartCoroutine(newState);
        }

        private bool AreBothPlayersReady()
        {
            foreach (LocalPlayerSO localPlayer in localPlayers)
            {
                if (!localPlayer.IsReady()) return false;
            }
            return true;
        }
    }
}