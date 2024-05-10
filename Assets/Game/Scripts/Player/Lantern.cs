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
        [SerializeField] public float burnSpeed;
        [SerializeField] public float lanternEffectiveRadius;
        [SerializeField] public float lanternPower;
        [SerializeField] public Light2D lightConfig;

        public void DecreaseFuel(float dt)
        {
            currentFuel -= burnSpeed * dt;
            currentFuel = Mathf.Clamp(currentFuel, 0, capacity);
        }

        public float AddFuel(float amount)
        {
            float emptySpace = capacity - currentFuel;

            currentFuel = Mathf.Clamp(currentFuel + amount,
                currentFuel,
                capacity);

            float leftover = Mathf.Clamp(amount - emptySpace, 0, amount);
            return leftover;
        }

        public bool IsEmpty => currentFuel <= 0.001f;
    }

    public class Lantern : MonoBehaviour
    {
        [SerializeField] private List<Stage> stages;
        [SerializeField] private Light2D emptyLanternConfig;
        [SerializeField] private Transform hand;
        [SerializeField] private PlayerMovementRb player;
        [SerializeField] private float lanternRotationAngle = -15f;
        [SerializeField] private float lanternRotationSpeedMultiplier = 10f;
        [SerializeField] private float lanternReturnSpeed = 5f;
        [SerializeField] private List<SpriteRenderer> lanternParts;
        [SerializeField] private Light2D lanternLight;

        private readonly Stage _emptyStage = new() { capacity = 1f };
        private float _currentLanternRotation;
        public event Action OnEmptied;
        public event Action OnAddFuel;
        public event Action<int> OnStageChanged;

        private int _stageId;
        private Stage _stage;
        private bool _isActive;
        private bool _isEmpty;
        public float Fuel => _stage.currentFuel;
        public float StageCapacity => _stage.capacity;
        public bool IsEmpty => _isEmpty;

        public int StageId => _stageId;
        public float LanternRadius => _isEmpty ? 0 : _stage.lanternEffectiveRadius;
        public float LanternPower => _isEmpty ? 0 : _stage.lanternPower;

        private void Awake()
        {
            _stageId = stages.Count - 1;
            _stage = stages[_stageId];

            SetLight(_stage.lightConfig);
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

        private void SetLight(Light2D config)
        {
            lanternLight.color = config.color;
            lanternLight.intensity = config.intensity;
            lanternLight.pointLightInnerRadius = config.pointLightInnerRadius;
            lanternLight.pointLightOuterRadius = config.pointLightOuterRadius;
            lanternLight.falloffIntensity = config.falloffIntensity;
            lanternLight.shadowIntensity = config.shadowIntensity;
        }


        private void UseFuel()
        {
            if (_isActive && !_isEmpty)
                _stage.DecreaseFuel(Time.deltaTime);
            UpdateStage();
        }


        private void UpdateStage()
        {
            if (!_stage.IsEmpty) return;
            if (_stageId == 0)
            {
                if (_isEmpty)
                    return;

                SetLight(emptyLanternConfig);
                _isEmpty = true;
                _stage = _emptyStage;
                OnEmptied?.Invoke();

                return;
            }

            _stageId--;
            OnStageChanged?.Invoke(_stageId);
            _stage = stages[_stageId];
            SetLight(_stage.lightConfig);
        }

        private void DisableLight()
        {
            lanternLight.intensity = 0;
        }

        public void AddFuel(float amount)
        {
            if (amount > 0.01f) _isEmpty = false;

            int idx = _stageId;
            float leftover = amount;
            while (idx < stages.Count && leftover >= 0.001f)
            {
                _stageId = idx;
                leftover = stages[idx].AddFuel(leftover);
                idx++;
                Debug.Log($"{leftover} : {idx}");
            }

            OnAddFuel?.Invoke();
            _stage = stages[_stageId];
            SetLight(_stage.lightConfig);
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