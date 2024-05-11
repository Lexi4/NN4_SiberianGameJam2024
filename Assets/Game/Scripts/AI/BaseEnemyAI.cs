using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Game.Scripts;
using Game.Scripts.Player;
using Pathfinding;
using Pathfinding.Util;
using UnityEngine;

namespace AI
{
    public class BaseEnemyAI : MonoBehaviour
    {
        [SerializeField] public float AttackRange;
        [SerializeField] public float AttackDuration;
        
        public IAstarAI AI;
        public List<ILightHolder> possibleTargets;
        public List<ILightHolder> activeTargets;
        public ILightHolder target;
        
        protected IEnumerator defaultBehaviour;

        public BaseEnemyAI()
        {
            possibleTargets = new List<ILightHolder>();
            activeTargets = new List<ILightHolder>();
        }

        protected virtual void Update()
        {
            //Update active targets (not event based now)
            activeTargets.ClearFast();
            foreach (var possibleTarget in possibleTargets)
            {
                if (possibleTarget != null && possibleTarget.GetActivePower() > 0)
                    activeTargets.Add(possibleTarget);
            }
        }

        private void OnEnable()
        {
            AI = GetComponent<IAstarAI>();
        }

        private void OnDisable()
        {
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
            if(newTarget != null) 
                possibleTargets.Add(newTarget);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var newTarget = other.gameObject.GetComponentInChildren<ILightHolder>();
            if(newTarget != null) 
                possibleTargets.Remove(newTarget);
        }

        protected IEnumerator Behaviour_Attack()
        {
            if (target == null)
            {
                yield break;
            }
            
            target.TakeDamage();
            yield return new WaitForSeconds(AttackDuration);
        }

        protected virtual void OnFlash()
        {
            throw new NotImplementedException();
        }
    }
}