using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public event Action onInteract;
        public event Action onFlash;
        private PlayerInputActions _playerInputActions;
        private bool _isRunning;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }

        private void Start()
        {
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.Run.started += OnRunStarted;
            _playerInputActions.Player.Run.canceled += OnRunEnded;
            _playerInputActions.Player.Interact.performed += OnInteract;
            _playerInputActions.Player.Flash.performed += OnFlash;
            _playerInputActions.Player.FireLantern.performed += OnFireLantern;
        }

        private void OnFireLantern(InputAction.CallbackContext obj)
        {
        }

        private void OnFlash(InputAction.CallbackContext obj)
        {
            onFlash?.Invoke();
        }

        private void OnInteract(InputAction.CallbackContext obj)
        {
            onInteract?.Invoke();
        }

        private void OnRunEnded(InputAction.CallbackContext obj)
        {
            _isRunning = false;
        }

        private void OnRunStarted(InputAction.CallbackContext obj)
        {
            _isRunning = true;
        }

        public bool IsRunning() => _isRunning;

        public float GetMoveInput()
        {
            return _playerInputActions.Player.Movement.ReadValue<float>();
        }
    }
}