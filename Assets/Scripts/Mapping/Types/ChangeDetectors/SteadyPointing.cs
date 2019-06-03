using System.Diagnostics;
using Effectors;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Mapping.Types.ChangeDetectors
{
    public class SteadyPointing : ChangeDetector
    {
        private LineRenderer _lineRenderer;
        private Transform _transform;
        private int _layerMask;
        private Stopwatch _stopwatch;
        private int _debouncer = 10;
        private readonly int focusTime = 500;


        public SteadyPointing(GameObject pointer)
        {
            // Bit shift the index of the layer (9) to get a bit mask
            _layerMask = 1 << 9;
            _transform = pointer.transform;
            _stopwatch = new Stopwatch();

            _lineRenderer =  pointer.transform.GetComponentInParent<LineRenderer>();
            _lineRenderer.startWidth = 0.05f;
            _lineRenderer.endWidth = 0.05f;
        }

        /// <summary>
        /// Checks whether the position of the end effector is supposed to induce changes.
        /// </summary>
        /// <returns>result should be from [-10;10]</returns>
        public float IsChanging()
        {
            _lineRenderer.SetPosition(0,_transform.position);
            _lineRenderer.SetPosition(1,(-_transform.up) * 1000);
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
                if (_stopwatch.Elapsed.TotalMilliseconds <= focusTime) return 0;
                
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
    }
}