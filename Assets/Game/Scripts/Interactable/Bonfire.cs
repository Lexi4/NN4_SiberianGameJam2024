using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.Scripts.Interactable
{
    public class Bonfire : MonoBehaviour, ILightHolder
    {
        [Header("Logic")] [SerializeField] private float effectiveRadius;
        [SerializeField] private float cost;
        [SerializeField] private float burnSpeed;
        [SerializeField] private float fuelCapacity;
        [SerializeField] private int power;
        private float _fuel;

        [Header("View")] [SerializeField] private Light2D lightSource;
        [SerializeField] private Light2D activeLight, inactiveLight;
        [SerializeField] private float switchTime = 1f;
        [SerializeField] private GameObject activeState, inactiveState;
        [SerializeField] private bool isActiveFromTheStart;

        public bool IsActive { get; private set; }

        private void Awake()
        {
            SetLight(isActiveFromTheStart ? activeLight : inactiveLight);

            if (isActiveFromTheStart)
            {
                IsActive = true;
                _fuel = fuelCapacity;
                activeState.SetActive(true);
                inactiveState.SetActive(false);
            }
            else
            {
                IsActive = false;
                activeState.SetActive(false);
                inactiveState.SetActive(true);
                _fuel = 0;
            }
        }

        private void Update()
        {
            if (!IsActive) return;


            _fuel -= burnSpeed * Time.deltaTime;
            _fuel = Mathf.Clamp(_fuel, 0, _fuel);
            if (_fuel < 0.001f)
            {
                SetInactive();
            }
        }

        public void SetActive()
        {
            if (IsActive)
                return;

            IsActive = true;
            _fuel = fuelCapacity;
            StartCoroutine(SwitchLightRoutine(inactiveLight, activeLight,
                inactiveState, activeState));
        }

        private void SetInactive()
        {
            if (!IsActive)
                return;

            IsActive = false;
            _fuel = 0;
            StartCoroutine(SwitchLightRoutine(activeLight, inactiveLight,
                activeState, inactiveState));
        }

        private IEnumerator SwitchLightRoutine(Light2D from, Light2D to, GameObject before, GameObject after)
        {
            before.SetActive(false);
            after.SetActive(true);
            float t = 0;
            while (t <= switchTime)
            {
                t += Time.deltaTime;
                InterpolateBetweenLights(from, to, t / switchTime, lightSource);
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

        private void SetLight(Light2D config)
        {
            lightSource.color = config.color;
            lightSource.intensity = config.intensity;
            lightSource.pointLightInnerRadius = config.pointLightInnerRadius;
            lightSource.pointLightOuterRadius = config.pointLightOuterRadius;
            lightSource.falloffIntensity = config.falloffIntensity;
            lightSource.shadowIntensity = config.shadowIntensity;
        }

        public float Cost => cost;
        
        public void TakeDamage()
        {
            SetInactive();
        }

        public float GetActiveRadius() => effectiveRadius;

        public int GetActivePower() => IsActive ? power : 0;

        public Vector3 GetPosition() => transform.position;
    }
}