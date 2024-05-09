using System.Collections;
using UnityEngine;

namespace AI
{
    public class EnemyRegularAI : BaseEnemyAI
    {
        public EnemyRegularAI()
        {
            defaultBehaviour = Behaviour_Patrol();
        }
        private void OnTriggerEnter (Collider other)
        {
            Debug.Log ($"Collided with: {other.gameObject.name}");
            
            if (!other.gameObject.CompareTag("Player")) return;
            target = other.gameObject;
            ForceBehaviour(Behaviour_Chasing());
        }

        private void OnTriggerStay (Collider other)
        {
            Debug.Log ($"A {other.gameObject.name} is inside the DoorObject trigger");
        }

        private void OnTriggerExit (Collider other)
        {
            Debug.Log ($"A {other.gameObject.name} has exited the DoorObject trigger");
            ForceBehaviour(Behaviour_Patrol());
        }

        public IEnumerator Behaviour_Patrol()
        {
            //Walk around. Search something (player)
            yield break;
        }
        
        public IEnumerator Behaviour_RunAway()
        {
            //Play anim. Find target position away from light 
            yield break;
        }
        
        public IEnumerator Behaviour_Chasing()
        {
            //Go for player
            
            yield break;
        }
    }
}