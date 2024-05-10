using System;
using System.Collections;
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

        public float BurnFuel(float dt)
        {
            currentFuel -= burnSpeed * dt;
            currentFuel = Mathf.Clamp(currentFuel, 0, capacity);
            return currentFuel;
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

        public float DecreaseFuel(float amount)
        {
            float leftover = Mathf.Clamp(amount - currentFuel, 0, amount);
            currentFuel -= amount;
            currentFuel = Mathf.Clamp(currentFuel, 0, capacity);

            return leftover;
        }


        public bool IsEmpty => currentFuel <= 0.001f;
    }

    public class Lantern : MonoBehaviour
    {
        [Header("    Light Config")] [SerializeField]
        private List<Stage> stages;

        [SerializeField] private Stage emptyStage;
        [SerializeField] private Light2D lanternLight;
        [Header("Flash")] [SerializeField] private float flashCost;
        [SerializeField] private float flashAnimTime = 0.4f;
        [SerializeField] private Light2D flashLight;

        [Header("    Movement reaction")] [SerializeField]
        private Transform hand;

        [SerializeField] private PlayerMovementRb player;
        [SerializeField] private float lanternRotationAngle = -15f;
        [SerializeField] private float lanternRotationSpeedMultiplier = 10f;
        [SerializeField] private float lanternReturnSpeed = 5f;
        [SerializeField] private List<SpriteRenderer> lanternParts;


        private float _currentLanternRotation;
        public event Action OnEmptied;
        public event Action onFlashUsed;
        public event Action<int> OnStageChanged;
        private Light2D CurrentStageLight => _stage.lightConfig;
        private int _stageId;
        private Stage _stage;
        private bool _isActive;
        private bool _isEmpty;
        public int StageCount => stages.Count;
        public bool IsEmpty => _isEmpty;


        public float LanternRadius => _isEmpty ? 0 : _stage.lanternEffectiveRadius;
        public float LanternPower => _isEmpty ? 0 : _stage.lanternPower;

        public float GetStageFuelNormalized(int idx)
        {
            if (idx < stages.Count && idx >= 0)
            {
                return stages[idx].currentFuel / stages[idx].capacity;
            }

            return 0;
        }

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
                _stage.BurnFuel(Time.deltaTime);
            UpdateStage();
        }


        private void UpdateStage()
        {
            if (!_stage.IsEmpty) return;
            if (_stageId == 0)
            {
                if (_isEmpty)
                    return;

                _isEmpty = true;
                _stage = emptyStage;
                SetLight(_stage.lightConfig);
                OnEmptied?.Invoke();

                return;
            }

            _stageId--;
            OnStageChanged?.Invoke(_stageId);
            _stage = stages[_stageId];
            SetLight(_stage.lightConfig);
        }


        public void AddFuel(float amount)
        {
            int idx = _stageId;
            float leftover = amount;
            while (idx < stages.Count && leftover >= 0.001f)
            {
                _stageId = idx;
                leftover = stages[idx].AddFuel(leftover);
                idx++;
            }

            _stage = stages[_stageId];
            SetLight(_stage.lightConfig);
            if (_stage.currentFuel >= 0.001f)
            {
                _isEmpty = false;
            }
        }

        public void UseFlash()
        {
            if (_isEmpty)
                return;
            DecreaseFuel(flashCost);
            onFlashUsed?.Invoke();
            StartCoroutine(FlashAnimRoutine());
        }

        private IEnumerator FlashAnimRoutine()
        {
            float t = 0;
            SetLight(flashLight);
            while (t <= flashAnimTime)
            {
                t += Time.deltaTime;
                InterpolateBetweenLights(flashLight, CurrentStageLight, t / flashAnimTime, lanternLight);
                yield return null;
            }
        }

        private void InterpolateBetweenLights(Light2D from, Light2D to, float t, Light2D dist)
        {
            dist.color = Color.Lerp(from.color, to.color, t);
            dist.intensity = Mathf.Lerp(from.intensity, to.intensity, t);
            dist.pointLightInnerRadius = Mathf.Lerp(from.pointLightInnerRadius, to.pointLightInnerRadius, t);
            dist.pointLightOuterRadius = Mathf.Lerp(from.pointLightOuterRadius, to.pointLightOuterRadius, t);
            dist.falloffIntensity = Mathf.Lerp(from.falloffIntensity, to.falloffIntensity, t);
            dist.shadowIntensity = Mathf.Lerp(from.shadowIntensity, to.shadowIntensity, t);
        }

        private void DecreaseFuel(float amount)
        {
            int idx = _stageId;
            float leftover = amount;
            while (idx >= 0 && leftover >= 0.001f)
            {
                _stageId = idx;
                leftover = stages[idx].DecreaseFuel(leftover);
                if (leftover >= 0.001f)
                    idx--;
            }

            if (idx < 0)
            {
                _isEmpty = true;
                _stage = emptyStage;
                SetLight(_stage.lightConfig);
                OnEmptied?.Invoke();
            }
            else
            {
                _stage = stages[_stageId];
                SetLight(_stage.lightConfig);
            }
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

            if (Mathf.Abs(_currentLanternRotation - desAngle) <= 0.01f) 
                _currentLanternRotation = desAngle;
            
            _currentLanternRotation = Mathf.Lerp(_currentLanternRotation, desAngle, Mathf.Abs(amount));
            hand.transform.rotation = quaternion.Euler(0, 0, _currentLanternRotation * Mathf.Deg2Rad);
        }
    }
}