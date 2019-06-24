using System.Globalization;
using Rokoko.Smartsuit;
using UnityEngine;

namespace Assessment
{
    public class FacingChecker
    {
        private Camera _suit;
        private GameObject _startArea;
        private Vector3 _forward;

        public FacingChecker(Camera suit, GameObject startArea)
        {
            _suit = suit;
            _startArea = startArea;
            _forward = _startArea.transform.transform.forward;
        }

        public bool InTheArea()
        {
            return _startArea.GetComponent<Collider>().bounds.Contains(_suit.gameObject.transform.position);
        }

        public bool FacingForward()
        {
            return Vector3.Angle(_forward, _suit.gameObject.transform.forward) < 20;
        }
    }
}