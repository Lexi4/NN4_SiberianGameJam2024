using System.Collections;
using Game.Scripts;
using Game.Scripts.Player;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class EnemyRegularAI : BaseEnemyAI
    {
        [Header("Behaviour Patrol")] [SerializeField]
        private float patrolRadius = 2.0f;

        [SerializeField] private float patrolInterval = 2.5f;

        public EnemyRegularAI()
        {
            defaultBehaviour = Behaviour_Patrol();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            target = other.gameObject;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            target = null;
        }

        public IEnumerator Behaviour_Patrol()
        {
            while (!target)
            {
                //Don't do anything 3 seconds but keep eye on target
                yield return new WaitForSecondsOrInterrupt(patrolInterval, () => target);

                //Walk around. Search something (player)
                var randomOffset = math.remap(0.0f, 1.0f, -patrolRadius, patrolRadius, Random.value);
                ai.destination = Utils.PosToGround(ai.position + (Vector3.right * randomOffset));

                yield return null;
            }

            StartCoroutine(Behaviour_Chasing());
        }

        public IEnumerator Behaviour_RunAway()
        {
            //Play anim. Find target position away from light
            while (target)
            {
                var lantern = target.GetComponentInChildren<Lantern>();
                var awayVector = (ai.position - target.transform.position);

                var remainAwayDist = lantern.lanternRadius - awayVector.magnitude;
                var runAwayDestination = ai.position + (awayVector.normalized * remainAwayDist);
                ai.destination = Utils.PosToGround(runAwayDestination);

                //Todo: write despawn logic and send event to NPC system to spawn new in other position
                yield return null;
            }

            StartCoroutine(Behaviour_Chasing());

            yield break;
        }

        public IEnumerator Behaviour_Chasing()
        {
            while (target)
            {
                var distanceToTarget = (target.transform.position - ai.position).magnitude;
                var lantern = target.GetComponentInChildren<Lantern>();
                if (distanceToTarget < lantern.lanternRadius && lantern.lanternPower > 0)
                {
                    StartCoroutine(Behaviour_RunAway());
                    yield break;
                }

                ai.destination = Utils.PosToGround(target.transform.position);
                yield return null;
            }

            ai.destination = ai.position;

            StartCoroutine(Behaviour_Patrol());
        }
    }
}