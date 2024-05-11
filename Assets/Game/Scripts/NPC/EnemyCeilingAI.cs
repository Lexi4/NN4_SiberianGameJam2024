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
            Target = UtilsAI.GetNearestActiveTarget(this);
        }

        public IEnumerator Behaviour_Patrol()
        {
            while (Target == null)
            {
                AI.destination = AI.position;
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
                    AI.destination = Utils.PosToGround(Target.GetPosition());
                    animController.MovingAnimation = IsMoving;
                    
                    if (UtilsAI.CanAttackTarget(this, Target))
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