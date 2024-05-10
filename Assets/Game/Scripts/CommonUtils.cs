using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class Utils
    {
        public static Vector3 PosToGround(Vector3 source)
        {
            var origin = source;
            origin.y += 1.0f;
            
            var Hits = Physics2D.RaycastAll(origin, Vector3.down, float.PositiveInfinity, LayerMask.GetMask("Obstacle"));

            foreach (var hit in Hits)
                if (hit.collider)
                    return hit.point;

            return source;
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