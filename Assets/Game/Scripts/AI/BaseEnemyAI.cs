using System.Collections;
using UnityEngine;

namespace AI
{
    public class BaseEnemyAI : MonoBehaviour
    {
        [SerializeField] protected float patrolRadius = 2.0f;
        [SerializeField] protected IEnumerator defaultBehaviour;
        
        protected GameObject target = null;

        protected virtual void Start()
        {
            StartCoroutine(defaultBehaviour);
        }
        
        //Should used only outside behaviour methods
        public void ForceBehaviour(IEnumerator behaviour)
        {
            StopAllCoroutines();
            StartCoroutine(behaviour);
        }

        protected virtual void OnDestroy()
        { 
            StopAllCoroutines();
        }
    }
}