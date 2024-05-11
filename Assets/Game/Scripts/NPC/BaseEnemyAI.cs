using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Game.Scripts;
using Game.Scripts.Player;
using Pathfinding;
using Pathfinding.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI
{
    public class BaseEnemyAI : MonoBehaviour
    {
        protected int IsMoving => Convert.ToInt32(!(AI.reachedDestination || AI.reachedEndOfPath));

        [Header("Attack Behaviour")]
        [SerializeField] public float AttackRange;
        [SerializeField] public float AttackDuration;
        
        public IAstarAI AI;
        public EnemyAnimationController animController;
        public List<ILightHolder> PossibleTargets;
        public List<ILightHolder> ActiveTargets;
        public ILightHolder Target;
        
        protected IEnumerator defaultBehaviour;

        public BaseEnemyAI()
        {
            PossibleTargets = new List<ILightHolder>();
            ActiveTargets = new List<ILightHolder>();
        }

        protected virtual void Update()
        {
            //Update active targets (not event based now)
            UpdateTargets();
        }

        private void OnEnable()
        {
            AI = GetComponent<IAstarAI>();
            animController = GetComponent<EnemyAnimationController>();
        }

        private void OnDisable()
        {
            animController = null;
            AI = null;
        }

        protected virtual void Start()
        {
            StartCoroutine(defaultBehaviour);
            
            var lantern = World.Get().Player.GetComponentInChildren<Lantern>();
            if(lantern)
                lantern.OnFlashUsed += OnFlashUsedHandler;
        }
        
        private void OnFlashUsedHandler(object sender, Lantern.FlashUsedEventArgs args)
        {
            if ((args.position - AI.position).magnitude <= args.radius)
                OnFlash();
        }

        //Should used only outside behaviour methods
        public void ChangeBehaviour(IEnumerator behaviour)
        {
            StopAllCoroutines();
            StartCoroutine(behaviour);
        }

        protected virtual void OnDestroy()
        { 
            StopAllCoroutines();

            var world = World.Get();
            if (world == null || world.Player == null) 
                return;
            var lantern = world.Player.GetComponentInChildren<Lantern>();
            if(lantern) 
                lantern.OnFlashUsed -= OnFlashUsedHandler;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var newTarget = other.gameObject.GetComponentInChildren<ILightHolder>();
            if (newTarget != null)
            {
                PossibleTargets.Add(newTarget);
                UpdateTargets();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var newTarget = other.gameObject.GetComponentInChildren<ILightHolder>();
            if (newTarget != null)
            {
                PossibleTargets.Remove(newTarget);
                UpdateTargets();
            }
        }

        protected virtual void UpdateTargets()
        {
            ActiveTargets.ClearFast();
            foreach (var possibleTarget in PossibleTargets)
            {
                if (possibleTarget != null && possibleTarget.GetActivePower() > 0)
                    ActiveTargets.Add(possibleTarget);
            }
        }

        protected IEnumerator Behaviour_Attack()
        {
            if (Target == null)
            {
                yield break;
            }
            animController.AttackAnimation = 1;
            Target.TakeDamage();
        }

        protected virtual void OnFlash()
        {
            throw new NotImplementedException();
        }
    }
}