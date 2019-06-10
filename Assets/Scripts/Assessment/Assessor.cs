using System.Collections.Generic;
using System.IO;
using Effectors;
using UnityEngine;

namespace Assessment
{
    public class Assessor: MonoBehaviour
    {
        private readonly Dictionary<EndEffector, List<Result>> _effectors = new Dictionary<EndEffector, List<Result>>();
        private string path = "result.csv";
        
        public void AddEffector(EndEffector effector)
        {
            _effectors[effector] = new List<Result>();
            effector.Initialise(this);
        }

        public void AddResult(Result result, EndEffector efector )
        {
            if (_effectors.ContainsKey(efector))
            {
                _effectors[efector].Add(result);
            }
        }

        public void SaveResult()
        {
            var file = File.CreateText(path);
            var line = string.Join(",", "Effector", "Mapping type", " Target depth", "Target size", "Movement Time");
            file.WriteLine(line);
            foreach (var effectorResult in _effectors)
            {
                var result = effectorResult.Value;
                var effector = effectorResult.Key;
               
                foreach (var arr in result)
                {
                    line = string.Join(",",effector.name, arr.mappingType, arr.targetDepth, arr.targetSize, arr.movementTime);
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