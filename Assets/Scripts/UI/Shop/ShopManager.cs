using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Balltallion.UI.Shop
{
    public class ShopManager : MonoBehaviour
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