using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.Scripts.Player
{
    public class Lantern : MonoBehaviour
    {
        [SerializeField] private float fuel = 100;
        [SerializeField] private float fuelBurnSpeed = 0.1f;
        [SerializeField] private Transform hand;
        [SerializeField] private PlayerMovementRb player;
        [SerializeField] private float lanternRotationAngle = -15f;
        [SerializeField] private float lanternRotationSpeedMultiplier = 10f;
        [SerializeField] private float lanternReturnSpeed = 5f;
        [SerializeField] private List<SpriteRenderer> lanternParts;
        [SerializeField] private Light2D lanternLight;
        private float _currentLanternRotation;

        private void Start()
        {
            player.OnMove += OnMove;
        }

        private void Update()
        {
            if (player.IsSprinting)
                Hide();
            else
                Show();

            fuel -= fuelBurnSpeed * Time.deltaTime;
        }

        private void Show()
        {
            lanternLight.enabled = true;
            foreach (var part in lanternParts) part.enabled = true;
        }

        private void Hide()
        {
            lanternLight.enabled = false;
            foreach (var part in lanternParts) part.enabled = false;
        }

        private void OnMove(float amount)
        {
            RotateLantern(amount);
        }

        private void RotateLantern(float amount)
        {
            float desAngle = lanternRotationAngle * Mathf.Sign(amount);
            if (Mathf.Abs(amount) < 0.001f)
            {
                desAngle = 0;
                amount = lanternReturnSpeed * Time.deltaTime;
            }
            else
            {
                amount *= lanternRotationSpeedMultiplier;
            }

            if (Mathf.Abs(_currentLanternRotation - desAngle) <= 0.01f) _currentLanternRotation = desAngle;
            _currentLanternRotation = Mathf.Lerp(_currentLanternRotation, desAngle, Mathf.Abs(amount));

            hand.transform.rotation = quaternion.Euler(0, 0, _currentLanternRotation * Mathf.Deg2Rad);
        }
    }
}