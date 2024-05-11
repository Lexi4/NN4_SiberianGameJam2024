using System;
using System.Collections.Generic;
using AI;
using Game.Scripts.Player;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class Utils
    {
        public static Vector3 PosToGround(Vector3 source)
        {
            var origin = source;
            origin.y += 1.0f;
            
            var hits = Physics2D.RaycastAll(origin, Vector3.down, float.PositiveInfinity, LayerMask.GetMask("Obstacle"));

            foreach (var hit in hits)
                if (hit.collider)
                    return hit.point;

            return source;
        }
    }

    public class UtilsAI
    {
        public static Vector3 GetNextPatrolPoint(IAstarAI ai, float radius)
        {
            var randomOffset = math.remap(0.0f, 1.0f, -radius, radius, Random.value);
            var result = Utils.PosToGround(ai.position + (Vector3.right * randomOffset));
            return result;
        }

        public static Vector3 GetRunAwayPoint(IAstarAI ai, ILightHolder target, float distanceFromLaternEdge)
        {
            var awayVector = (ai.position - target.GetPosition());
            awayVector.y = 0;

            var remainAwayDist = target.GetActiveRadius() - awayVector.magnitude;
            var runAwayDestination = ai.position + (awayVector.normalized * (remainAwayDist * distanceFromLaternEdge));
            
            runAwayDestination.y += 4.0f;
            runAwayDestination = Utils.PosToGround(runAwayDestination);
            return runAwayDestination;
        }

        public static bool IsAffectedByLightHolder(IAstarAI ai, ILightHolder target)
        {
            var distanceToTarget = (target.GetPosition() - ai.position).magnitude;
            var bUnderLantern = distanceToTarget < target.GetActiveRadius() && target.GetActivePower() > 0;
            return bUnderLantern;
        }

        private static Lantern _latern;
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static Lantern GetLatern(GameObject target)
        {
            if (!_latern)
            {
                _latern = target.GetComponentInChildren<Lantern>();
            }
            return _latern;
        }

        public static ILightHolder GetNearestTarget(BaseEnemyAI self)
        {
            var result = GetNearestTargetInList(self.AI.position, self.PossibleTargets);
            return result;
        }
        
        public static ILightHolder GetPlayerTarget(BaseEnemyAI self)
        {
            foreach (var possibleTarget in self.PossibleTargets)
            {
                if (((MonoBehaviour)possibleTarget).gameObject.CompareTag("Player"))
                    return possibleTarget;
            }
            return null;
        }
        
        public static ILightHolder GetNearestActiveTarget(BaseEnemyAI self)
        {
            var result = GetNearestTargetInList(self.AI.position, self.ActiveTargets);
            return result;
        }
        
        public static ILightHolder GetNearestTargetInList(Vector3 position, List<ILightHolder> list)
        {
            if (list.Count == 0)
            {
                return null;
            }

            ILightHolder nearest = null;
            float minMagnitude = float.PositiveInfinity;
            foreach (ILightHolder target in list)
            {
                var targetMagnitude = (target.GetPosition() - position).magnitude; 
                if (minMagnitude > targetMagnitude)
                {
                    minMagnitude = targetMagnitude;
                    nearest = target;
                }
            }
            return nearest;
        }

        public static bool CanAttackTarget(BaseEnemyAI self, ILightHolder target)
        {
            var targetMagnitude = (target.GetPosition() - self.AI.position).magnitude;
            return targetMagnitude <= self.AttackRange;
        }
    }

    public class WaitForSecondsOrInterrupt : CustomYieldInstruction
    {
        private float _time;
        private readonly float _waitTime;
        private readonly Func<bool> _interrupt;

        public override bool keepWaiting
        {
            get
            {
                _time += Time.deltaTime;
                return _time < _waitTime && !(_interrupt != null && _interrupt.Invoke());
            }
        }

        public WaitForSecondsOrInterrupt(float waitTime, Func<bool> interrupt)
        {
            _time = 0.0f;
            _waitTime = waitTime;
            _interrupt = interrupt;
        }
    }
}