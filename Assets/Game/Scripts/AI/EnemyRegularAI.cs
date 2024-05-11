using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

namespace AI
{
    public class EnemyRegularAI : BaseEnemyAI
    {
        [Header("Behaviour Patrol")]
        [SerializeField] private float patrolRadius = 2.0f;
        [SerializeField] private float patrolInterval = 2.5f;
        [SerializeField] private float attackDuration = 0.5f;
        [SerializeField] private float stunDuration = 3.0f;

        public EnemyRegularAI()
        {
            defaultBehaviour = Behaviour_Patrol();
        }

        protected override void Update()
        {
            base.Update();
            target = UtilsAI.GetPlayerTarget(this);
        }

        protected new void OnFlash()
        {
            ChangeBehaviour(Behaviour_Stun());
        }

        public IEnumerator Behaviour_Patrol()
        {
            while (target == null)
            {
                //Don't do anything 3 seconds but keep eye on target
                yield return new WaitForSecondsOrInterrupt(patrolInterval, () => target != null);
                AI.destination = UtilsAI.GetNextPatrolPoint(AI, patrolRadius);

                yield return null;
            }

            StartCoroutine(Behaviour_Chasing());
        }

        public IEnumerator Behaviour_RunAway()
        {

            while (target != null)
            {
                AI.destination = UtilsAI.GetRunAwayPoint(AI, target, 5);
                yield return null;
            }

            StartCoroutine(Behaviour_Chasing());
        }

        public IEnumerator Behaviour_Chasing()
        {
            while (target != null)
            {
                if (UtilsAI.IsAffectedByLightHolder(AI, target))
                {
                    StartCoroutine(Behaviour_RunAway());
                    yield break;
                }

                AI.destination = Utils.PosToGround(target.GetPosition());
                
                if (UtilsAI.CanAttackTarget(this, target))
                {
                    target.TakeDamage();
                    yield return new WaitForSeconds(attackDuration);
                }

                yield return null;
            }
            AI.destination = AI.position;

            StartCoroutine(Behaviour_Patrol());
        }

        public IEnumerator Behaviour_Stun()
        {
            AI.destination = AI.position;
            yield return new WaitForSeconds(stunDuration);
            StartCoroutine(Behaviour_Patrol());
        }
    }
}