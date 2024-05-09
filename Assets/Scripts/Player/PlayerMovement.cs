using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")] [SerializeField, Range(0, 20)]
        private float maxWalkSpeed;

        [SerializeField] private AnimationCurve walkSpeedCurve;
        [SerializeField] private float walkAccelerationTime;
        [SerializeField, Range(0, 20)] private float maxSprintSpeed;
        [SerializeField] private AnimationCurve sprintSpeedCurve;
        [SerializeField] private float sprintAccelerationTime;

        [Header("Lantern")] [SerializeField] private GameObject body;
        [SerializeField] private GameObject lantern;
        [SerializeField] private Transform leftLanternPlaceholder, rightLanternPlaceholder;
        [SerializeField] private float maxLanternRotation = 15f;
        [SerializeField] private float lanternReturnSpeedMultiplier = 10f;
        [SerializeField] private float lanternRotationSpeedMultiplier = 10f;
        [SerializeField] private float lanternReturnSpeed = 5f;
        private PlayerInput _playerInput;
        private bool _isOnFullWalkSpeed;
        private bool _isSprinting;
        private float _currentSpeed;
        private float _prevInput;
        private float _tWalk, _tSprint;
        private float Input => _playerInput.GetMoveInput();

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            float amount = Move();
            RotateLantern(amount);
            UpdateLanternState();
            Flip();
        }

        private float Move()
        {
            float moveAmount = GetSpeed() * Time.deltaTime * Input;
            Vector3 newPos = transform.position + Vector3.right * moveAmount;
            transform.position = newPos;
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


        private void RotateLantern(float amount)
        {
            Quaternion desiredRot;
            if (Mathf.Abs(amount) < 0.001f)
            {
                desiredRot = Quaternion.Euler(0, 0, 0);
                amount = lanternReturnSpeed * Time.deltaTime * lanternReturnSpeedMultiplier;
            }
            else
            {
                desiredRot = Quaternion.Euler(0, 0,
                    -maxLanternRotation * Mathf.Sign(amount));

                amount *= lanternRotationSpeedMultiplier;
            }

            lantern.transform.rotation = Quaternion.Slerp(
                lantern.transform.rotation,
                desiredRot,
                Mathf.Abs(amount));
        }

        private void UpdateLanternState() => lantern.SetActive(!_isSprinting);

        private void Flip()
        {
            if (Input == 0)
                return;
            bool isMoveLeft = Input < 0;
            body.GetComponent<SpriteRenderer>().flipX = isMoveLeft;

            if (isMoveLeft)
                lantern.transform.position = leftLanternPlaceholder.position;
            else
                lantern.transform.position = rightLanternPlaceholder.position;
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