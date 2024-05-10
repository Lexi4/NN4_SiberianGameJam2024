using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.Scripts.Player
{
    [Serializable]
    class Stage
    {
        [SerializeField] public float capacity;
        [SerializeField] public float currentFuel;

        public void DecreaseFuel(float amount)
        {
            currentFuel -= amount;
            currentFuel = Mathf.Clamp(currentFuel, 0, capacity);
        }

        public bool IsEmpty => currentFuel <= 0.001f;
    }

    public class Lantern : MonoBehaviour
    {
        [SerializeField] private List<Stage> stages;
        [SerializeField] private float fuelBurnSpeed = 0.1f;
        [SerializeField] private Transform hand;
        [SerializeField] private PlayerMovementRb player;
        [SerializeField] private float lanternRotationAngle = -15f;
        [SerializeField] private float lanternRotationSpeedMultiplier = 10f;
        [SerializeField] private float lanternReturnSpeed = 5f;
        [SerializeField] private List<SpriteRenderer> lanternParts;
        [SerializeField] private Light2D lanternLight;

        private float _currentLanternRotation;
        public event Action onEmptied;
        public event Action<int> onStageChanged;

        private int _stageId;
        private float _totalFuel;
        private Stage _stage;
        private bool _isActive;
        private bool _isEmpty;

        private void Awake()
        {
            _stageId = stages.Count - 1;
            _stage = stages[_stageId];
            foreach (var stage in stages)
            {
                _totalFuel += stage.capacity;
            }

            UpdateStage();
        }

        private void Start()
        {
            player.OnMove += OnMove;
        }

        private void Update()
        {
            if (player.IsRunning)
                Hide();
            else
                Show();

            UseFuel();
        }

        public float Fuel => _stage.currentFuel;
        public float StageCapacity => _stage.capacity;
        public bool IsEmpty => _isEmpty;

        public int StageId => _stageId;

        private void UseFuel()
        {
            if (_isActive && !_isEmpty)
                _stage.DecreaseFuel(fuelBurnSpeed * Time.deltaTime);
            UpdateStage();
        }

        private void AddFuel(float amount)
        {
            _isEmpty = false;
        }

        private void UpdateStage()
        {
            if (!_stage.IsEmpty) return;
            if (_stageId - 1 < 0)
            {
                _isEmpty = true;
                onEmptied?.Invoke();
                return;
            }

            _stageId--;
            onStageChanged?.Invoke(_stageId);
            _stage = stages[_stageId];
        }

        private void Show()
        {
            _isActive = true;
            lanternLight.enabled = true;
            foreach (var part in lanternParts) part.enabled = true;
        }

        private void Hide()
        {
            _isActive = false;
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