using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Effectors;
using Rokoko.Smartsuit;
using Rokoko.Smartsuit.Commands;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using UnityEngine.Experimental.LowLevel;

namespace Assessment
{
    public class Assessor: MonoBehaviour
    {
        private readonly Dictionary<EndEffector, List<Result>> _effectors = new Dictionary<EndEffector, List<Result>>();
        private string effortBaselinePath = "effortBaselineResult.csv";
        private StreamWriter baselineSW;
        private SmartsuitActor suit;
        private Academy _academy;
        private TargetSpawner _targetSpawner;
        private string _playerName;
        public Try currentTry;
        private bool _gotNeutral;
        private Text _text;
        private FacingChecker _facingChecker;
        private int countdown = 3;
        private bool startedCounting = false;

        
        

        private int _currentBatch;
        private List<Result> _batchResults;
        public int TargetsTapped = 0;

        private void Start()
        {
            _academy = FindObjectOfType<Academy>();
            _playerName = _academy.PlayerIndex;
            _targetSpawner = FindObjectOfType<TargetSpawner>();
            var actor = FindObjectsOfType<AnhaActor>().First(a => a.name == "CURRENT");
            suit = actor.actor;
            _gotNeutral = false;
            actor.GetNeutralPosition();
            _currentBatch = 0;
            TargetsTapped = 0;
            _batchResults = new List<Result>();
            currentTry = gameObject.AddComponent<Try>();    
            currentTry.Initialise(suit, _targetSpawner.Target, _effectors.Keys.ToList());
            
            _text = FindObjectOfType<Text>();
            _facingChecker = new FacingChecker(suit, GameObject.FindWithTag("Start"));
        }

        private void Update()
        {
            if (!currentTry.IsRunning && _gotNeutral)
            {
                if (!startedCounting)
                {
                    if (!_facingChecker.InTheArea())
                        _text.text = "Please go to the start area";
                    else if (!_facingChecker.FacingForward())
                    {
                        _text.text = "Please turn to the playing area";
                        countdown = 4;
                    }
                    else
                    {
                        StartCoroutine(nameof(LoseTime));
                        startedCounting = true;
                    }
                }
                _text.text = "" + countdown;
                if(countdown == 0)
                {
                    StartNewTry();
                    startedCounting = false;
                    _text.text = "";
                }
            }
            
        }

        public void AddEffector(EndEffector effector)
        {
            _effectors[effector] = new List<Result>();
            effector.Initialise(this);
        }
        
        // get results for a batch 
        public List<Result> GetResult(int batchSize)
        {
            var res = _batchResults;        
            _batchResults = new List<Result>();
            return  res;
            
        }
        
        public void AddResult(Result result, EndEffector effector )
        {
            if (_effectors.ContainsKey(effector))
            {
                _effectors[effector].Add(result);
            }
            _batchResults.Add(result);
            TargetsTapped++;
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

        public void StartNewTry()
        {
            var position = _targetSpawner.GetNewPosition();
            var scale = _targetSpawner.GetNewScale();
            var id = _targetSpawner.CurrentTarget;
            currentTry.StartNewTry(_playerName, id.ToString(), position, scale);
        }

        public void StopTry()
        {
            currentTry.StopTry();
            countdown = 3;
        }
        
        IEnumerator LoseTime()
        {
            while (countdown >= 0) {
                yield return new WaitForSeconds (1);
                countdown--; 
            }
        }
        
    }
    
}