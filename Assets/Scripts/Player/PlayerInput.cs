using UnityEngine;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        
        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }

        private void Start()
        {
            _playerInputActions.Player.Enable();
        }

        public float GetMoveInput()
        {
            return _playerInputActions.Player.Movement.ReadValue<float>();
        }
    }
}