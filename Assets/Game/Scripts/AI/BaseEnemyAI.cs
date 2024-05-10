using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

namespace AI
{
    public class BaseEnemyAI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] protected AIPath aiPath;
        [SerializeField] protected Collider2D viewingZone;
        protected IAstarAI ai;
        protected GameObject target = null;
        
        protected IEnumerator defaultBehaviour;

        private void OnEnable()
        {
            ai = GetComponent<IAstarAI>();
        }

        private void OnDisable()
        {
            ai = null;
        }

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