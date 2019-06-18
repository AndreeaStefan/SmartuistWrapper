using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Effectors;
using Rokoko.Smartsuit;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Assessment
{
    public class Try : MonoBehaviour
    {
        private int _everyFrame = 10;
        [FormerlySerializedAs("_enabled")][HideInInspector] public bool IsRunning;
        private string _playerName;
        private string _id;
        private Vector3 _position;
        private Vector3 _scale;
        private StreamWriter _writer;
        private SmartsuitActor _suit;
        private GameObject _target;
        private Stopwatch _stopwatch;
        private EndEffector _effector1;
        private EndEffector _effector2;
        
        private string _resultPath = "tapResult.csv";
        private string _effortPath = "effortResult.csv";
        private StreamWriter _effortSW;
        private StreamWriter _tapSW;


        public void Initialise(SmartsuitActor sa, GameObject target, List<EndEffector> effectors)
        {
            _effortSW = new StreamWriter(_effortPath, true);
            _tapSW = new StreamWriter(_resultPath, true);
            _effector1 = effectors.First(e => e.name.Contains("Left"));
            _effector2 = effectors.First(e => e.name.Contains("Right"));
            
            _suit = sa;
            IsRunning = false;
            _target = target;
            _stopwatch = new Stopwatch();
        }

        private void Update()
        {
            if(IsRunning && _everyFrame == 0)
            {
                _everyFrame = 10;
                var tmp = Helper.GetPositionRecord(_suit.CurrentState);
                var tmpPos = _target.transform.position;
                var distanceToMain = Vector3.Distance(_suit.transform.position, tmpPos);
                var distanceToLeft = Vector3.Distance(_effector1.transform.position,tmpPos );
                var distanceToRight = Vector3.Distance(_effector2.transform.position, tmpPos);
                var line = $"{_playerName},{_id},{distanceToMain},{distanceToLeft},{distanceToRight},{tmp}\n";
                _effortSW.Write(line);
            }
            if(IsRunning)
                _everyFrame--;
        }


        public void StartNewTry(string playerName, string targetID, Vector3 pos, Vector3 scale)
        {
            IsRunning = true;
            _playerName = playerName;
            _id = targetID;
            _position = pos;
            _scale = scale;

            _target.transform.position = _position;
            _target.transform.localScale = _scale;
            _target.GetComponent<MeshRenderer>().enabled = true;
            _stopwatch.Start();
        }

        public void StopTry()
        {
            IsRunning = false;
            _target.GetComponent<MeshRenderer>().enabled = false;
            _stopwatch.Stop();
            
            var line = $"{_playerName},{_id},{_position},{_scale},{_stopwatch.Elapsed.TotalMilliseconds}\n";
            _tapSW.Write(line);
            
            _stopwatch.Reset();
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Application ending  " );
            _tapSW.Close();
            _effortSW.Close();
        }
    }
}