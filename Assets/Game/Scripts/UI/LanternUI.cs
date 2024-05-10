using System;
using System.Collections.Generic;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class LanternUI : MonoBehaviour
    {
        [SerializeField] private List<Image> stages;
        private Image _stage;
        private Lantern _lantern;

        private float GetNormalizedFuel()
            => _lantern.Fuel / _lantern.StageCapacity;

        private void Awake()
        {
            _lantern = GameObject.FindWithTag("Lantern").GetComponent<Lantern>();
        }

        private void Start()
        {
            _stage = stages[_lantern.StageId];
            _lantern.onStageChanged += OnStageChanged;
        }

        private void OnStageChanged(int stageId)
        {
            if (stageId < stages.Count && stageId >= 0)
            {
                _stage = stages[stageId];
                EmptyUsedStages();
            }
        }

        private void EmptyUsedStages()
        {
            for (int i = stages.IndexOf(_stage) + 1; i < stages.Count; i++)
                stages[i].fillAmount = 0;
        }

        private void Update()
        {
            float fill = GetNormalizedFuel();
            if (Mathf.Abs(fill) <= 0.0025f)
                fill = 0;

            _stage.fillAmount = fill;
        }
    }
}