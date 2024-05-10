using System;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerMovementRb : MonoBehaviour
    {
        [Header("        Movement")] 
        [Header("Walk")] 
        [SerializeField, Range(0, 20)] private float maxWalkSpeed;

        [SerializeField] private AnimationCurve walkSpeedCurve;
        [SerializeField] private float walkAccelerationTime;

        [Header("Run")] 
        [SerializeField, Range(0, 20)] private float maxRunSpeed;
        [SerializeField] private AnimationCurve runSpeedCurve;
        [SerializeField] private float runAccelerationTime;

        [Header("Other")] [SerializeField] private GameObject body;

        public event Action<float> OnMove;
        private Rigidbody2D _rb;
        private PlayerInput _playerInput;
        private bool _isOnFullWalkSpeed;
        private bool _isRunning;
        private float _currentSpeed;
        private float _prevInput;
        private float _tWalk, _tRun;

        private float _currentLanternRotation;
        private readonly Quaternion _leftRot = Quaternion.Euler(0, 180, 0);
        private readonly Quaternion _rightRot = Quaternion.Euler(0, 0, 0);
        private float Input => _playerInput.GetMoveInput();
        public Vector2 CurrentVelocity => _rb.velocity;
        public float CurrentSpeed => _currentSpeed;
        public bool IsRunning => _isRunning;

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

            _isRunning = false;
            if (isInputZero || isInputDifferent) // stand still or changed direction
            {
                ResetMoving();
            }
            else if (!_playerInput.IsRunning() || !_isOnFullWalkSpeed) // only walking
            {
                _tRun = 0;
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
            else if (_playerInput.IsRunning() && _isOnFullWalkSpeed) // running
            {
                _tRun += Time.deltaTime;
                _isRunning = true;
                if (_tRun > runAccelerationTime)
                    _tRun = runAccelerationTime;

                _currentSpeed = GetRunningSpeed(_tRun);

                if (Mathf.Abs(_currentSpeed - maxRunSpeed) <= 0.001f)
                    _currentSpeed = maxRunSpeed;
            }


            return _currentSpeed;
        }

        private void ResetMoving()
        {
            _currentSpeed = 0;
            _tWalk = 0;
            _tRun = 0;
            _isOnFullWalkSpeed = false;
        }

        private float GetRunningSpeed(float t) =>
            maxWalkSpeed + runSpeedCurve.Evaluate(t / runAccelerationTime) *
            (maxRunSpeed - maxWalkSpeed);

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
            if (maxRunSpeed < maxWalkSpeed)
            {
                maxRunSpeed = maxWalkSpeed + 0.01f;
            }
        }
    }
}