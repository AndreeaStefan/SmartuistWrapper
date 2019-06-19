using System.Collections;
using System.Collections.Generic;
using System.IO;
using Connection;
using Effectors;
using Rokoko.Smartsuit;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;

namespace Assessment
{
    public class Assessor: MonoBehaviour
    {
        private readonly Dictionary<EndEffector, List<Result>> _effectors = new Dictionary<EndEffector, List<Result>>();
        private string resultPath = "result.csv";
        private string effortPath = "effortResult.csv";
        private StreamWriter effortSW;
        private string effortBaselinePath = "effortBaselineResult.csv";
        private StreamWriter baselineSW;
        private SmartsuitActor suit;
        private Academy _academy;
        private TargetSpawner _targetSpawner;
        private string _playerName;
        private int frame = 10;
        private DataForwarder _dataForwarder;

        private int _currentBatch;
        private List<Result> _batchResults;
        public int TargetsTapped = 0;

        private void Start()
        {
//            _dataForwarder = new DataForwarder();
            _playerName = FindObjectOfType<Academy>().PlayerIndex;
            _targetSpawner = FindObjectOfType<TargetSpawner>();
            var actor = FindObjectOfType<AnhaActor>();
            suit = actor.actor;
            actor.GetNeutralPosition();
            baselineSW = new StreamWriter(effortBaselinePath, true);
            effortSW = new StreamWriter(effortPath, true);

            _currentBatch = 0;
            TargetsTapped = 0;
            _batchResults = new List<Result>();
        }

        public void AddEffector(EndEffector effector)
        {
            _effectors[effector] = new List<Result>();
            effector.Initialise(this);
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

        public void SaveResult()
        {
            effortSW.Close();
            
            var file = File.CreateText(resultPath);
//            var line = string.Join(",", "Player", "target","Effector", "Mapping type", " Target depth", "Target size", "Movement Time");
//            file.WriteLine(line);
            foreach (var effectorResult in _effectors)
            {
                var result = effectorResult.Value;
                var effector = effectorResult.Key;
               
                foreach (var arr in result)
                {
                    var line = string.Join(",", _playerName,arr.targetIndex ,effector.name, arr.mappingType, arr.targetDepth, arr.targetSize, arr.movementTime);
                    file.WriteLine(line);
                }
                
            }
            file.Close();
        }

        // get results for a batch 
        public List<Result> GetResult(int batchSize)
        {
            var res = _batchResults;        
            _batchResults = new List<Result>();
            return  res;
            
        }

        private void Update()
        {
            if (frame == 0)
            {
                frame = 10;
                var tmp = Helper.GetPositionRecord(suit.CurrentState);
                var line = $"{_playerName},{_targetSpawner.CurrentTarget},{tmp}\n";
//                _dataForwarder.Send(line);
                effortSW.Write(line);
            }
            frame--;
        }


        void OnApplicationQuit()
        {
            Debug.Log("Application ending  " );
            SaveResult();

        }


        public void SaveBaselineRecord(string record)
        {
            baselineSW.WriteLine($"{_playerName},{record}");
        }

        public IEnumerator FinaliseSavingPosition(float time)
        {
            yield return new WaitForSeconds(time);
            baselineSW.Close();
        }
    }
    
}