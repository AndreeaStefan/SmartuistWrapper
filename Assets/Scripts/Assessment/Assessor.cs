using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Assessment.Results;
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
        private Academy _academy;
        private TargetSpawner _targetSpawner;
        private Text _text;
        private FacingChecker _facingChecker;

        private string _playerName;
       
        private int _countdown = 3;
        private Stopwatch _stopwatch;
        
        private int _currentRepetition;
        private List<RepetitionResult> _previousBatchResults;
        private List<RepetitionResult> _currentResults;
        private bool _startedCounting;
        private bool _gotNeutral;
        private bool _alreadyStopped;


        private Target _currTarget;
        private GradientDescent _gradientDescent;

        [FormerlySerializedAs("currentTry")] public Lesson currentLesson;
        [Range(1, 15)] public int BatchSize = 5;
        public int CurrentLessonNr;
        public Camera camera;
        public float ParticipantWeight;
        public bool Male;
        
        public float Scale;

        private void Start()
        {
            EffortResult.SetStats(ParticipantWeight, Male);
            _stopwatch = new Stopwatch();
            _academy = FindObjectOfType<Academy>();
            _playerName = _academy.PlayerIndex;
            _targetSpawner = FindObjectOfType<TargetSpawner>();
            _player = FindObjectsOfType<AnhaActor>().First(a => a.name == "CURRENT");
            _tapSW = new StreamWriter(_resultPath, true);
       
            // todo kill it when we won't need the neutral position
            _gotNeutral = true;
//            actor.GetNeutralPosition();
            CurrentLessonNr = 0;
            _currentRepetition = -1;
            _previousBatchResults = new List<RepetitionResult>();
            _currentResults = new List<RepetitionResult>();
            currentLesson = gameObject.AddComponent<Lesson>();    
            currentLesson.Initialise(_player, _targetSpawner.Target, _effectors, CurrentLessonNr, _currentRepetition);
            
            _text = FindObjectOfType<Text>();
            _text = FindObjectOfType<Text>();
            _facingChecker = new FacingChecker(camera, GameObject.FindWithTag("Start"));
            Scale = 1;
            _gradientDescent = new GradientDescent(1, BatchSize);      
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
            _alreadyStopped = false;
            _targetSpawner.GetNewPosition();
           _targetSpawner.GetNewScale();
           _currTarget = _targetSpawner.CurrentTarget;
           _currentRepetition++;
           
            currentLesson.StartNewTry(_playerName, CurrentLessonNr, _currentRepetition, _currTarget.Position, _currTarget.Scale);
            _stopwatch.Start();    
        }

        public void StopRepetition()
        {
            if (!_alreadyStopped)
            {
                _alreadyStopped = true;

                _stopwatch.Stop();
                currentLesson.StopTry();

                var result = new RepetitionResult(_playerName, CurrentLessonNr, _currentRepetition,
                    _targetSpawner.CurrentTarget.ID,
                    _targetSpawner.CurrentTarget.Scale, _targetSpawner.CurrentTarget.Position,
                    _player.PreviousPosition.position, _targetSpawner.CurrentTarget.Angle,
                    _stopwatch.ElapsedMilliseconds, EffortResult.GetNewResult());

                _tapSW.WriteLine(result.ToString() + "," + Scale + "," + _gradientDescent.Loss);
                _tapSW.Flush();

                _currentResults.Add(result);

                if (BatchSize == _currentRepetition)
                {
                    _currentRepetition = 0;
                    CurrentLessonNr++;
                    Scale = _gradientDescent.GetNextScale(_currentResults);


                    _previousBatchResults = _currentResults;
                    _currentResults = new List<RepetitionResult>();
                }
                _countdown = 3;
                _stopwatch.Reset();
            }
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