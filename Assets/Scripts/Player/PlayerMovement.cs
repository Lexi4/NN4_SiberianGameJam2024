using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Range(0, 20)] private float speed;
        [SerializeField] private GameObject body;
        [SerializeField] private GameObject lantern;
        [SerializeField] private Transform leftLanternPlaceholder, rightLanternPlaceholder;
        [SerializeField] private float maxLanternRotation = 45f;
        [SerializeField] private float lanternReturnSpeedMultiplier = 10f;
        [SerializeField] private float lanternRotationSpeedMultiplier = 10f;
        private PlayerInput _playerInput;

        private float Input => _playerInput.GetMoveInput();

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }


        private void Update()
        {
            Move();
            Flip();
        }

        private void Move()
        {
            float moveAmount = speed * Time.deltaTime * Input;
            Vector3 newPos = transform.position + Vector3.right * moveAmount;
            RotateLantern(moveAmount);
            transform.position = newPos;
        }

        private void RotateLantern(float amount)
        {
            Quaternion desiredRot;
            if (Mathf.Abs(amount) < 0.001f)
            {
                desiredRot = Quaternion.Euler(0, 0, 0);
                amount = speed * Time.deltaTime * lanternReturnSpeedMultiplier;
            }
            else
            {
                desiredRot = Quaternion.Euler(0, 0, -maxLanternRotation * Mathf.Sign(amount));
                amount *= lanternRotationSpeedMultiplier;
            }

            lantern.transform.rotation = Quaternion.Slerp(lantern.transform.rotation,
                desiredRot,
                Mathf.Abs(amount));
        }

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
    }
}