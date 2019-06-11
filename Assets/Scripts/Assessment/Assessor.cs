using System.Collections.Generic;
using System.IO;
using Effectors;
using UnityEngine;

namespace Assessment
{
    public class Assessor: MonoBehaviour
    {
        private readonly Dictionary<EndEffector, List<Result>> _effectors = new Dictionary<EndEffector, List<Result>>();
        private string resultPath = "result.csv";
        private string effortPath = "result.csv";
        private Academy _academy;
        private AnhaActor _actor;
        private string _playerName;

        private void Start()
        {
            _playerName = FindObjectOfType<Academy>().PlayerIndex;
            _actor = FindObjectOfType<AnhaActor>();
            _actor.GetNeutralPosition();
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
        }

        public void SaveResult()
        {
            var file = File.CreateText(resultPath);
            var line = string.Join(",", "Player","Effector", "Mapping type", " Target depth", "Target size", "Movement Time");
            file.WriteLine(line);
            foreach (var effectorResult in _effectors)
            {
                var result = effectorResult.Value;
                var effector = effectorResult.Key;
               
                foreach (var arr in result)
                {
                    line = string.Join(",", _playerName,effector.name, arr.mappingType, arr.targetDepth, arr.targetSize, arr.movementTime);
                    file.WriteLine(line);
                }
                
            }
            file.Close();
        }

        void OnApplicationQuit()
        {
            Debug.Log("Application ending  " );
            SaveResult();

        }
    }
    
}