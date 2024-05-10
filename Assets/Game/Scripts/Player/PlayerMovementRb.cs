using System;
using Player;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerMovementRb : MonoBehaviour
    {
        [Header("Movement")] 
        [SerializeField, Range(0, 20)] private float maxWalkSpeed;
        [SerializeField] private AnimationCurve walkSpeedCurve;
        [SerializeField] private float walkAccelerationTime;
        [SerializeField, Range(0, 20)] private float maxSprintSpeed;
        [SerializeField] private AnimationCurve sprintSpeedCurve;
        [SerializeField] private float sprintAccelerationTime;

        [Header("Other")] 
        [SerializeField] private GameObject body;

        public event Action<float> OnMove;
        private Rigidbody2D _rb;
        private PlayerInput _playerInput;
        private bool _isOnFullWalkSpeed;
        private bool _isSprinting;
        private float _currentSpeed;
        private float _prevInput;
        private float _tWalk, _tSprint;

        private float _currentLanternRotation;
        private readonly Quaternion _leftRot = Quaternion.Euler(0, 180, 0);
        private readonly Quaternion _rightRot = Quaternion.Euler(0, 0, 0);
        private float Input => _playerInput.GetMoveInput();
        public Vector2 CurrentVelocity => _rb.velocity;
        public bool IsSprinting => _isSprinting;
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _rb = GetComponent<Rigidbody2D>();
        }


        private void FixedUpdate()
        {
            float amount = Move();
            OnMove?.Invoke(amount);
            Flip();
        }

        private float Move()
        {
            float velX = GetSpeed();
            float velY = _rb.velocity.y;

            if (velY > 0) velY *= 0.55f;

            _rb.velocity = new Vector2(velX * Input, velY);
            float moveAmount = velX * Time.fixedDeltaTime * Input;
            _prevInput = Input;
            return moveAmount;
        }


        private float GetSpeed()
        {
            bool isInputDifferent =
                (int)Mathf.Round(Mathf.Sign(_prevInput)) != (int)Mathf.Round(Mathf.Sign(Input));
            bool isInputZero = Mathf.Abs(Input) <= 0.01f;

            _isSprinting = false;
            if (isInputZero || isInputDifferent) // stand still or changed direction
            {
                ResetMoving();
            }
            else if (!_playerInput.IsSprinting() || !_isOnFullWalkSpeed) // only walking
            {
                _tSprint = 0;
                _tWalk += Time.deltaTime;

                if (_tWalk > walkAccelerationTime)
                    _tWalk = walkAccelerationTime;

                _currentSpeed = GetWalkSpeed(_tWalk);

                if (Mathf.Abs(_currentSpeed - maxWalkSpeed) <= 0.001f)
                {
                    _currentSpeed = maxWalkSpeed;
                    _isOnFullWalkSpeed = true;
                }
            }
            else if (_playerInput.IsSprinting() && _isOnFullWalkSpeed) // sprinting
            {
                _tSprint += Time.deltaTime;
                _isSprinting = true;
                if (_tSprint > sprintAccelerationTime)
                    _tSprint = sprintAccelerationTime;

                _currentSpeed = GetSprintSpeed(_tSprint);

                if (Mathf.Abs(_currentSpeed - maxSprintSpeed) <= 0.001f)
                    _currentSpeed = maxSprintSpeed;
            }


            return _currentSpeed;
        }

        private void ResetMoving()
        {
            _currentSpeed = 0;
            _tWalk = 0;
            _tSprint = 0;
            _isOnFullWalkSpeed = false;
        }

        private float GetSprintSpeed(float t) =>
            maxWalkSpeed + sprintSpeedCurve.Evaluate(t / sprintAccelerationTime) *
            (maxSprintSpeed - maxWalkSpeed);

        private float GetWalkSpeed(float t)
            => walkSpeedCurve.Evaluate(t / walkAccelerationTime) * maxWalkSpeed;


    

        private void Flip()
        {
            if (Input == 0)
                return;
            bool isMoveLeft = Input < 0;

            body.transform.rotation = isMoveLeft ? _leftRot : _rightRot;
        }

        private void OnValidate()
        {
            if (maxSprintSpeed < maxWalkSpeed)
            {
                maxSprintSpeed = maxWalkSpeed + 0.01f;
            }
        }
    }
}