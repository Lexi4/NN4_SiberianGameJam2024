using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private bool _isSprinting;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }

        private void Start()
        {
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.Sprint.started += OnSprintStarted;
            _playerInputActions.Player.Sprint.canceled += OnSprintEnded;
            _playerInputActions.Player.Jump.performed += OnJumpPerformed;
        }

        private void OnJumpPerformed(InputAction.CallbackContext obj)
        {
           
        }

        private void OnSprintEnded(InputAction.CallbackContext obj)
        {
            _isSprinting = false;
        }

        private void OnSprintStarted(InputAction.CallbackContext obj)
        {
            _isSprinting = true;
        }

        public bool IsSprinting() => _isSprinting;

        public float GetMoveInput()
        {
            return _playerInputActions.Player.Movement.ReadValue<float>();
        }
    }
}