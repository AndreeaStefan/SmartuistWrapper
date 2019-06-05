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
        private readonly int focusTime = 200;

        public WristMove(GameObject hand)
        {
            _hand = hand;
            _stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Checks whether the position of the end effector is supposed to induce changes.
        /// </summary>
        /// <returns>result should be from [-10;10]</returns>
        public float IsChanging()
        {

            var angleX = WrapAngle(_hand.transform.localRotation.eulerAngles.x);

           // UnityEngine.Debug.Log(angleX);

            if (angleX > 40)
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


            if (angleX < -10)
            {
                if (!_stopwatch.IsRunning)
                {
                    _debouncer = 10;
                    _stopwatch.Start();
                    return 0;
                }
                if (_stopwatch.Elapsed.TotalMilliseconds <= focusTime) return 0;
                return -3;
            }

            if (_debouncer == 0)
            {
                _stopwatch.Reset();
            }
            else if (_debouncer > 0)
                _debouncer--;

            return 0;
        }

        private  float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }
    }

 
}
