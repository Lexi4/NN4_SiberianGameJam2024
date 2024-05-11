using System.Collections;
using Game.Scripts;
using UnityEngine;

namespace AI
{
    public class EnemyCeilingAI : BaseEnemyAI
    {
        [SerializeField] private float attackDuration = 0.5f;

        public EnemyCeilingAI()
        {
            defaultBehaviour = Behaviour_Patrol();
        }

        protected override void Update()
        {
            base.Update();
            target = UtilsAI.GetNearestActiveTarget(this);
        }

        public IEnumerator Behaviour_Patrol()
        {
            while (target == null)
            {
                AI.destination = AI.position;
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
                    AI.destination = Utils.PosToGround(target.GetPosition());
                    
                    if (UtilsAI.CanAttackTarget(this, target))
                    {
                        StartCoroutine(Behaviour_Attack());
                        yield break;
                    }
                }
                yield return null;
            }
            StartCoroutine(Behaviour_Patrol());
        }
    }
}