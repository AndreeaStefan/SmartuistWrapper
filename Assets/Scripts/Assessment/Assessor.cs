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

        private Stopwatch _stopwatch;

        private int _currentRepetition;
        private List<RepetitionResult> _currentResults;
        private bool _alreadyStopped;


        private Target _currTarget;
        private GradientDescent _gradientDescent;

        [FormerlySerializedAs("currentTry")] public Lesson currentLesson;
        [Range(1, 15)] public int BatchSize = 5;
        public int CurrentLessonNr;
        public Camera camera;
        public float ParticipantWeight;
        public float ParticipantHeight = 1.7f;
        public bool Male;
        public int MaximumLessons = 30;

        public float Scale;

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

            CurrentLessonNr = 0;
            _currentRepetition = 0;
            _currentResults = new List<RepetitionResult>();
            currentLesson = gameObject.AddComponent<Lesson>();
            currentLesson.Initialise(_player, _targetSpawner.Target, _effectors, CurrentLessonNr, _currentRepetition);

            _facingChecker = FindObjectOfType<FacingChecker>();
            Scale = 1;
            _gradientDescent = new GradientDescent(1, BatchSize);
            StartCoroutine(nameof(StartFacingChecker));
        }

        private void Update()
        {
            if (CurrentLessonNr <= MaximumLessons)
            {
                if (currentLesson.IsRunning && _stopwatch.ElapsedMilliseconds > 10000)
                    EmergencyStop();
            }
            else
            {
                UIHandler.startDisplay("Done!\nThank you for participation!");
            }
        }

        public void AddEffector(EndEffector effector)
        {
            _effectors.Add(effector);
            effector.Initialise(this);
        }

        public void StartNewRepetition()
        {
            _alreadyStopped = false;
            if (BatchSize == _currentRepetition)
            {
                _currentRepetition = 0;
                CurrentLessonNr++;
                Scale = _gradientDescent.GetNextScale(_currentResults);
                _currentResults = new List<RepetitionResult>();
            }

            _targetSpawner.GetNewTarget();
            _currTarget = _targetSpawner.CurrentTarget;

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
                    _player.PreviousPosition, _targetSpawner.CurrentTarget.Angle,
                    _stopwatch.ElapsedMilliseconds, EffortResult.GetNewResult());

                // current Scale - prev Gain (what gave the curr Scale)  - Delta - between this Gain and prev One (the delta that gave the Scale) 
                _tapSW.WriteLine(result + "," + Scale + "," + _gradientDescent.Gain + "," + _gradientDescent.Delta) ;
                _tapSW.Flush();
                _currentResults.Add(result);

                _currentRepetition++;

                if (BatchSize == _currentRepetition)
                    _questionnaire.StartQuestionnaire();
                else
                    _facingChecker.ActivateCountdown(StartNewRepetition, 2);

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
                    _player.PreviousPosition, _targetSpawner.CurrentTarget.Angle,
                    10000, EffortResult.GetNewBadResult());

                _tapSW.WriteLine(result + "," + Scale + "," + _gradientDescent.Gain);
                _tapSW.Flush();

                _currentResults.Add(result);
                _currentRepetition++;
            }
            if (BatchSize == _currentRepetition)
                _questionnaire.StartQuestionnaire();
            else
                _facingChecker.ActivateCountdown(StartNewRepetition, 2);

            _stopwatch.Reset();
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
            _facingChecker.ActivateCountdown(StartNewRepetition, 2);
        }

        IEnumerator StartFacingChecker()
        {
                yield return new WaitForSeconds(1.5f);
                _facingChecker.ActivateCountdown(StartNewRepetition, 2);
        }
    }
}