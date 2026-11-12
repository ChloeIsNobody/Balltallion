using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Balltallion.Input
{
    public class VirtualCursorLinker : MonoBehaviour
    {
        [SerializeField] private LocalPlayerSO localPlayer;
        private VirtualMouseInput virtualCursor;

        private void Awake()
        {
            virtualCursor = GetComponent<VirtualMouseInput>();
        }
        
        private void Start()
        {
            if (!localPlayer.IsConnected()) return;
            InputActionAsset playerActions = localPlayer.GetInputRelay().GetActionAsset();
            InputAction moveAction = playerActions.FindAction("Move");
            InputAction interactAction = playerActions.FindAction("Interact");
            virtualCursor.stickAction = new InputActionProperty(moveAction);
            virtualCursor.leftButtonAction = new InputActionProperty(interactAction);
        }
    }
}