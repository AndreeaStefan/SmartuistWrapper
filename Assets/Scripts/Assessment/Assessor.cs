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
    public class Assessor : MonoBehaviour
    {
        private readonly List<EndEffector> _effectors = new List<EndEffector>();
        private string effortBaselinePath = "effortBaselineResult.csv";
        private StreamWriter baselineSW;
        private string _perceivedEffortPath = "perceivedEffort.csv";
        private StreamWriter _perceivedEffortSW;
        private string _resultPath = "tapResult.csv";
        private StreamWriter _tapSW;

        private AnhaActor _player;
        private Academy _academy;
        private TargetSpawner _targetSpawner;
        private Text _text;
        private FacingChecker _facingChecker;
        private Questionnaire _questionnaire;

        private string _playerName;

        private int _countdown = 3;
        private Stopwatch _stopwatch;

        private int _currentRepetition;
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
        private bool _finishedQuestion = true;

        private void Start()
        {
            EffortResult.SetStats(ParticipantWeight, Male);
            _stopwatch = new Stopwatch();
            _academy = FindObjectOfType<Academy>();
            _questionnaire = FindObjectOfType<Questionnaire>();
            _playerName = _academy.PlayerIndex;
            _targetSpawner = FindObjectOfType<TargetSpawner>();
            _player = FindObjectsOfType<AnhaActor>().First(a => a.name == "CURRENT");
            _tapSW = new StreamWriter(_resultPath, true);
            _perceivedEffortSW = new StreamWriter(_perceivedEffortPath, true);

            // todo kill it when we won't need the neutral position
            _gotNeutral = true;
//            actor.GetNeutralPosition();
            CurrentLessonNr = 0;
            _currentRepetition = -1;
            _currentResults = new List<RepetitionResult>();
            currentLesson = gameObject.AddComponent<Lesson>();
            currentLesson.Initialise(_player, _targetSpawner.Target, _effectors, CurrentLessonNr, _currentRepetition);

            _facingChecker = new FacingChecker(camera, GameObject.FindWithTag("Start"));
            Scale = 1;
            _gradientDescent = new GradientDescent(1, BatchSize);
        }

        private void Update()
        {
            if (!currentLesson.IsRunning && _gotNeutral && _finishedQuestion)
            {
                if (!_startedCounting)
                {
                    if (!_facingChecker.InTheArea())
                        UIHandler.startDisplay("Please go to the start area");
                    else if (!_facingChecker.FacingForward())
                    {
                        UIHandler.startDisplay("Please turn to the playing area");
                        _countdown = 3;
                    }
                    else
                    {
                        StartCoroutine(nameof(LoseTime));
                        _startedCounting = true;
                    }
                }
                else
                    UIHandler.startDisplay("" + _countdown);

                if (_countdown == 0)
                {
                    StartNewRepetition();
                    _startedCounting = false;
                    UIHandler.stopDisplaying();
                }
            }
            else if (currentLesson.IsRunning && _stopwatch.ElapsedMilliseconds > 10000)
            {
                EmergencyStop();
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
            _targetSpawner.GetNewTarget();
            _currTarget = _targetSpawner.CurrentTarget;
            _currentRepetition++;

            currentLesson.StartNewTry(_playerName, CurrentLessonNr, _currentRepetition, _currTarget.Position,
                _currTarget.Scale);
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

                // current Scale - prev Gain (what gave the curr Scale)  - Delta - between this Gain and prev One (the delta that gave the Scale) 
                _tapSW.WriteLine(result + "," + Scale + "," + _gradientDescent.Gain + "," + _gradientDescent.Delta) ;
                _tapSW.Flush();

                _currentResults.Add(result);

                if ((BatchSize - 1) == _currentRepetition)
                {
                    _currentRepetition = -1;
                    CurrentLessonNr++;
                    Scale = _gradientDescent.GetNextScale(_currentResults);

                    _currentResults = new List<RepetitionResult>();
                    _questionnaire.StartQuestionnaire();
                    _finishedQuestion = false;
                }

                _countdown = 3;
                _stopwatch.Reset();
            }
        }

        private void EmergencyStop()
        {
            currentLesson.StopTry();
            _alreadyStopped = true;
            _stopwatch.Stop();

            while (_currentRepetition < BatchSize)
            {
                var result = new RepetitionResult(_playerName, CurrentLessonNr, _currentRepetition,
                    _targetSpawner.CurrentTarget.ID,
                    _targetSpawner.CurrentTarget.Scale, _targetSpawner.CurrentTarget.Position,
                    _player.PreviousPosition.position, _targetSpawner.CurrentTarget.Angle,
                    10000, EffortResult.GetNewBadResult());

                _tapSW.WriteLine(result + "," + Scale + "," + _gradientDescent.Gain);
                _tapSW.Flush();

                _currentResults.Add(result);
                _currentRepetition++;
            }

            _currentRepetition = -1;
            CurrentLessonNr++;
            Scale = _gradientDescent.GetNextScale(_currentResults);
            
            _questionnaire.StartQuestionnaire();
            _finishedQuestion = false;

            _currentResults = new List<RepetitionResult>();
            _countdown = 3;
            _stopwatch.Reset();
        }


        IEnumerator LoseTime()
        {
            while (_countdown >= 0)
            {
                yield return new WaitForSeconds(1);
                _countdown--;
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Application ending  ");
            _tapSW.Close();
            _perceivedEffortSW.Close();
        }

        public void DoneQuestionnaire(string result)
        {
            _perceivedEffortSW.Write($"{_playerName},{CurrentLessonNr-1},{result}\n");
            _perceivedEffortSW.Flush();
            _finishedQuestion = true;
        }
    }
}