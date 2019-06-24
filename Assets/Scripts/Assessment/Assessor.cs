using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Assessment;
using Effectors;
using Rokoko.Smartsuit;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assessment
{
    public class Assessor: MonoBehaviour
    {
        private readonly List<EndEffector> _effectors = new List<EndEffector>();
        private string effortBaselinePath = "effortBaselineResult.csv";
        private StreamWriter baselineSW;
        private string _resultPath = "tapResult.csv";
        private StreamWriter _tapSW;

        private AnhaActor _player;
        private SmartsuitActor suit;
        private Academy _academy;
        private TargetSpawner _targetSpawner;
        private Text _text;
        private FacingChecker _facingChecker;

        private string _playerName;
        [FormerlySerializedAs("currentTry")] public Lesson currentLesson;
        private int _countdown = 3;
        private Stopwatch _stopwatch;

        [Range(1, 15)] public int BatchSize = 5;
        public int CurrentLessonNr;
        private int _currentRepetition;
        private List<RepetitionResult> _previousBatchResults;
        private List<RepetitionResult> _currentResults;
        private bool _startedCounting;
        private bool _gotNeutral;

        private Target _currTarget;


        public Camera camera;


        private void Start()
        {
            _stopwatch = new Stopwatch();
            _academy = FindObjectOfType<Academy>();
            _playerName = _academy.PlayerIndex;
            _targetSpawner = FindObjectOfType<TargetSpawner>();
            var actor = FindObjectsOfType<AnhaActor>().First(a => a.name == "CURRENT");
            _player = actor;
            _tapSW = new StreamWriter(_resultPath, true);
            suit = actor.actor;
            // todo kill it when we won't need the neutral position
            _gotNeutral = true;
//            actor.GetNeutralPosition();
            CurrentLessonNr = 0;
            _currentRepetition = 0;
            _previousBatchResults = new List<RepetitionResult>();
            _currentResults = new List<RepetitionResult>();
            currentLesson = gameObject.AddComponent<Lesson>();    
            currentLesson.Initialise(suit, _targetSpawner.Target, _effectors, CurrentLessonNr, _currentRepetition);
            
            _text = FindObjectOfType<Text>();
            _text = FindObjectOfType<Text>();
            _facingChecker = new FacingChecker(camera, GameObject.FindWithTag("Start"));
        }

        private void Update()
        {
            if (!currentLesson.IsRunning && _gotNeutral)
            {
                if (!_startedCounting)
                {
                    if (!_facingChecker.InTheArea())
                        _text.text = "Please go to the start area";
                    else if (!_facingChecker.FacingForward())
                    {
                        _text.text = "Please turn to the playing area";
                        _countdown = 3;
                    }
                    else
                    {
                        StartCoroutine(nameof(LoseTime));
                        _startedCounting = true;
                    }
                }
                else
                    _text.text = "" + _countdown;
                if(_countdown == 0)
                {
                    StartNewRepetition();
                    _startedCounting = false;
                    _text.text = "";
                }
            }
            
        }

        public void AddEffector(EndEffector effector)
        {
            _effectors.Add(effector);
            effector.Initialise(this);
        }
        
        // get results for a batch 
        public List<RepetitionResult> GetBatchResult()
        {
            return  _previousBatchResults;
        }

        public void SaveBaselineRecord(string record)
        {
            baselineSW.WriteLine($"{_playerName},{record}");
        }

        public IEnumerator FinaliseSavingPosition(float time)
        {
            yield return new WaitForSeconds(time);
            baselineSW.Close();
            _gotNeutral = true;
        }

        public void StartNewRepetition()
        {
            _targetSpawner.GetNewPosition();
           _targetSpawner.GetNewScale();
           _currTarget = _targetSpawner.CurrentTarget;
           _currentRepetition++;
           
            currentLesson.StartNewTry(_playerName, CurrentLessonNr, _currentRepetition, _currTarget.Position, _currTarget.Scale);
            _stopwatch.Start();    
        }

        public void StopRepetition()
        {
            _stopwatch.Stop();
            currentLesson.StopTry();
            var result = new RepetitionResult(_playerName, CurrentLessonNr, _currentRepetition, _targetSpawner.CurrentTarget.ID,
                _targetSpawner.CurrentTarget.Scale, _targetSpawner.CurrentTarget.Position, _player.PreviousPosition.position, _targetSpawner.CurrentTarget.Angle, _stopwatch.ElapsedMilliseconds);
            _currentResults.Add(result);
            
            if (BatchSize == _currentRepetition)
            {
                _currentRepetition = 0;
                CurrentLessonNr++;

                _previousBatchResults = _currentResults;
                _currentResults = new List<RepetitionResult>();
            }
            
            _tapSW.WriteLine(result.ToString());
            _tapSW.Flush();
            _countdown = 3;
            _stopwatch.Reset();
        }
        
        IEnumerator LoseTime()
        {
            while (_countdown >= 0) {
                yield return new WaitForSeconds (1);
                _countdown--; 
            }
        }
        
        private void OnApplicationQuit()
        {
            Debug.Log("Application ending  " );
            _tapSW.Close();
        }
    }
    
}