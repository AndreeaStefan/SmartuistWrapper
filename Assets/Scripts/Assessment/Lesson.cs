using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Assessment.Results;
using Effectors;
using Rokoko.Smartsuit;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Assessment
{
    public class Lesson : MonoBehaviour
    {
        private int _everyFrame = 3;
        [FormerlySerializedAs("_enabled")][HideInInspector] public bool IsRunning;
        private string _playerName;
        private Vector3 _position;
        private Vector3 _scale;
        private StreamWriter _writer;
        private AnhaActor _suit;
        private GameObject _target;
        private EndEffector _effectorLeft;
        private EndEffector _effectorRight;
        

        private string _effortPath = "effortResult.csv";
        private StreamWriter _effortSW;
        private int _repetition;
        private int _lesson;


        public void Initialise(AnhaActor sa, GameObject target, List<EndEffector> effectors, int lesson, int repetition)
        {
            _effortSW = new StreamWriter(_effortPath, true);
            _effectorLeft = effectors.First(e => e.name.Contains("Left"));
            _effectorRight = effectors.First(e => e.name.Contains("Right"));
            _lesson = lesson;
            _repetition = repetition;
            
            _suit = sa;
            IsRunning = false;
            _target = target;
        }

        private void Update()
        {
            if(IsRunning && _everyFrame == 0)
            {
                _everyFrame = 3;
                var result = FrameResult.GetNewFrame(_suit, _position, _effectorLeft, _effectorRight, _playerName,
                    _lesson, _repetition);
                FrameResultDb.AddResult(result);
                _effortSW.WriteLine(result.ToString());
                _effortSW.Flush();
                
            }
            if(IsRunning)
                _everyFrame--;
        }


        public void StartNewTry(string playerName, int lesson, int repetition, Vector3 pos, Vector3 scale)
        {
            IsRunning = true;
            _playerName = playerName;
            _position = pos;
            _scale = scale;
            _lesson = lesson;
            _repetition = repetition;

            _target.transform.position = _position;
            _target.transform.localScale = _scale;
            _target.GetComponent<MeshRenderer>().enabled = true;
            _target.transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine(nameof(TurnOffTheLight));
        }

        public void StopTry()
        {
            IsRunning = false;
            _target.GetComponent<MeshRenderer>().enabled = false;   
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Application ending  " );
            _effortSW.Close();
        }

        private IEnumerator TurnOffTheLight()
        {
            yield return new WaitForSeconds(0.2f);
            _target.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}