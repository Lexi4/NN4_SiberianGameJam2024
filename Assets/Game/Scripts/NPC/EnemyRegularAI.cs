using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC
{
    public class EnemyRegularAI : BaseEnemyAI
    {

        [Header("Behaviour Patrol")]
        [SerializeField] public float PatrolRadius = 2.0f;
        [SerializeField] public float PatrolInterval = 2.5f;
        [SerializeField] public float StunDuration = 3.0f;

        public EnemyRegularAI()
        {
            defaultBehaviour = Behaviour_Patrol();
        }

        protected override void UpdateTargets()
        {
            base.UpdateTargets();
            Target = UtilsAI.GetPlayerTarget(this);
        }

        protected override void OnFlash()
        {
            ChangeBehaviour(Behaviour_Stun());
        }

        public IEnumerator Behaviour_Patrol()
        {
            while (Target == null)
            {
                AI.destination = UtilsAI.GetNextPatrolPoint(AI, PatrolRadius);
                
                //Don't do anything 3 seconds but keep eye on target
                yield return new WaitForSecondsOrInterrupt(PatrolInterval, () =>
                {
                    animController.MovingAnimation = IsMoving;
                    return Target != null;
                });

                yield return null;
            }

            StartCoroutine(Behaviour_Chasing());
        }

        public IEnumerator Behaviour_RunAway()
        {
            while (Target != null)
            {
                if (Target.GetActivePower() < 1)
                {
                    break;
                }

                AI.destination = UtilsAI.GetRunAwayPoint(AI, Target, 0);

                animController.MovingAnimation = IsMoving;

                yield return null;
            }

            StartCoroutine(Behaviour_Chasing());
        }

        public IEnumerator Behaviour_Chasing()
        {

            while (Target != null)
            {
                if (UtilsAI.IsAffectedByLightHolder(AI, Target))
                {
                    StartCoroutine(Behaviour_RunAway());
                    yield break;
                }

                if (UtilsAI.CanAttackTarget(this, Target))
                {
                    StartCoroutine(Behaviour_Attack());
                    yield break;
                }

                AI.destination = Utils.PosToGround(Target.GetPosition());
                
                animController.MovingAnimation = IsMoving;
                
                yield return null;
            }
            StartCoroutine(Behaviour_Patrol());
        }

        public IEnumerator Behaviour_Stun()
        {
      
            AI.destination = AI.position;
            animController.StunAnimation = 1;
            yield return new WaitForSeconds(StunDuration);
            StartCoroutine(Behaviour_Patrol());
        }
    }
}