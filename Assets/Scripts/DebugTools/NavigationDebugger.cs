using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Balltallion.DebugTools
{
    public class NavigationDebugger : MonoBehaviour
    {
        [SerializeField] private Button defaultButton;

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
        }
    }
}