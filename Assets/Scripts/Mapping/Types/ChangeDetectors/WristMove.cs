using Mapping.Types.ChangeDetectors;
using System.Diagnostics;
using UnityEngine;


namespace Assets.Scripts.Mapping.Types.ChangeDetectors
{
    public class WristMove : ChangeDetector
    {
        private Stopwatch _stopwatch;
        private int _debouncer = 10;
        private GameObject _hand;
        private GameObject _parent;
        private readonly int focusTime = 200;

        public WristMove(GameObject hand, GameObject parent)
        {
            _hand = hand;
            _parent = parent;
            _stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Checks whether the position of the end effector is supposed to induce changes.
        /// </summary>
        /// <returns>result should be from [-10;10]</returns>
        public float IsChanging()
        {
            var fw = _parent.transform.up;
            var handFw = _hand.transform.up;

            var signeAngle = Vector3.SignedAngle(fw, handFw, -Vector3.up);

            UnityEngine.Debug.Log(signeAngle);

            if (signeAngle > 70 )
            {
                if (!_stopwatch.IsRunning)
                {
                    _debouncer = 10;
                    _stopwatch.Start();
                    return 0;
                }
                if (_stopwatch.Elapsed.TotalMilliseconds <= focusTime) return 0;

                return 2;
            }


            if (signeAngle < -35)
            {
                if (!_stopwatch.IsRunning)
                {
                    _debouncer = 10;
                    _stopwatch.Start();
                    return 0;
                }
                if (_stopwatch.Elapsed.TotalMilliseconds <= focusTime) return 0;
                return -2;
            }

            if (_debouncer == 0)
            {
                _stopwatch.Reset();
            }
            else if (_debouncer > 0)
                _debouncer--;

            return 0;
        }
    }
}
