using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class EnemyRegularAI : BaseEnemyAI
    {
        [SerializeField] private float patrolRadius = 2.0f;
        [SerializeField] private float patrolInterval = 2.5f;

        public EnemyRegularAI()
        {
            defaultBehaviour = Behaviour_Patrol();
        }
        private void OnTriggerEnter2D (Collider2D other)
        {
            Debug.Log ($"{gameObject.name} collided with {other.gameObject.name}");
            
            if (!other.gameObject.CompareTag("Player")) return;
                target = other.gameObject;
        }
        private void OnTriggerExit2D (Collider2D other)
        {
            Debug.Log ($"A {other.gameObject.name} has exited the {gameObject.name} trigger");
            if (!other.gameObject.CompareTag("Player")) return;
                target = null;
        }

        public IEnumerator Behaviour_Patrol()
        {
            //Walk around. Search something (player)
            while (!target)
            {
                yield return Behaviour_Idle(patrolInterval);
                
                var randomOffset = math.remap(0.0f, 1.0f, -patrolRadius, patrolRadius, Random.value);
                ai.destination = ai.position + (Vector3.right * randomOffset);

                yield return null;
            }
            StartCoroutine(Behaviour_Chasing());
            yield break;
        }
        
        public IEnumerator Behaviour_RunAway()
        {
            //Play anim. Find target position away from light 
            yield break;
        }
        
        public IEnumerator Behaviour_Chasing()
        {
            while (target)
            {
                ai.destination = target.transform.position;
                yield return null;
            }
            ai.destination = ai.position;

            StartCoroutine(Behaviour_Patrol());
        }
        
        public IEnumerator Behaviour_Idle(float waitTime)
        {
            float counter = 0f;
            while (counter < waitTime)
            {
                if (target)
                    ForceBehaviour(Behaviour_Chasing());

                counter += Time.deltaTime;
                yield return null;
            }
        }
    }
}