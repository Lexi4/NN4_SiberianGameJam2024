using System;
using System.Collections;
using UnityEngine;

namespace Game.Scripts
{
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