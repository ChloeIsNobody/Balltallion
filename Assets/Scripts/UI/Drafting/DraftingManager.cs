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