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
        
        private void Awake()
        {
            _lantern = GameObject.FindWithTag("Lantern").GetComponent<Lantern>();
            if (_lantern)
            {
                _update = true;
            }
        }

        private bool _update;

        private void Start()
        {
            if (!_update) return;

            _lantern.OnStageChanged += OnStageChanged;
            _lantern.OnEmptied += OnEmptied;
        }


        private void UpdateView()
        {
            for (int i = 0; i < _lantern.StageCount; i++)
                stages[i].fillAmount = _lantern.GetStageFuelNormalized(i);
        }


        private void Update()
        {
            if (!_update) return;
            
            UpdateView();
        }

        private void OnEmptied()
        {
            foreach (var stage in stages) 
                stage.fillAmount = 0f;
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
    }
}