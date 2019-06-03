using System.Diagnostics;
using Effectors;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Mapping.Types.ChangeDetectors
{
    public class SteadyPointing : ChangeDetector
    {
        private EndEffector _endEffector;
        private Transform _transform;
        private int _layerMask;
        private Stopwatch _stopwatch;
        private int _debouncer = 10;
        private readonly int focusTime = 500;


        public SteadyPointing(EndEffector effector)
        {
            _endEffector = effector;
            // Bit shift the index of the layer (9) to get a bit mask
            _layerMask = 1 << 9;
            _transform = effector.transform;
            _stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Checks whether the position of the end effector is supposed to induce changes.
        /// </summary>
        /// <returns>result should be from [-10;10]</returns>
        public float IsChanging()
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(_transform.position, -_transform.up, out hit, Mathf.Infinity, _layerMask))
            {
                if (!_stopwatch.IsRunning)
                {
                    _debouncer = 10;
                    _stopwatch.Start();
                    return 0;
                }

                if (_stopwatch.Elapsed.Milliseconds <= focusTime) return 0;
                
                
                Debug.DrawRay(_transform.position, -_transform.up * hit.distance,
                    Color.yellow);
                return 1.3f;

            }

            if (_debouncer == 0)
            {
                _stopwatch.Reset();
            }
            else if(_debouncer > 0)
                _debouncer--;
            
            Debug.DrawRay(_transform.position, -_transform.up * 100, Color.white);
            return -1.3f;
        }

        public void SetEndEffector(EndEffector endEffector)
        {
            _endEffector = endEffector;
        }
    }
}