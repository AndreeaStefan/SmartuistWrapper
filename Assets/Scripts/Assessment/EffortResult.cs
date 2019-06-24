using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Assessment
{
    public class EffortResult
    {
        private StreamReader _effortResults;
        private string fileName = "effortComputed.csv";
        private int _trackedBodyParts = 7;
        private float _participantWeight;
        private bool _isMale;
        private int _batchSize;

        private float[] _effortPerBodyPart;
        public int lesson;
        public int repetition;

        // body, leftUpperArm, RightUpperArm, RightFoot, LeftFoot, LeftHand, RightHand
        public float[] _weightPercentage;

        public EffortResult(float weight, bool isMale)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _effortResults = new StreamReader(fs);
            _effortResults.ReadToEnd();
            _effortPerBodyPart = new float[_trackedBodyParts];

            _participantWeight = weight;

            if(isMale)
                 _weightPercentage = new float[7] { 0.551f, 0.0325f, 0.0325f, 0.1668f, 0.1668f, 0.057f, 0.057f };
            else
                _weightPercentage = new float[7] { 0.532f, 0.029f, 0.029f, 0.1843f, 0.1843f, 0.0497f, 0.0497f };
        }

        public List<float[]> GetNewResult()
        {
            var results = new List<float[]>();
            var line = "";
            while ((line = _effortResults.ReadLine()) != null)
            {
                try
                {
                    _effortPerBodyPart = new float[_trackedBodyParts];
                    var res = line.Split(',');
                    lesson = Int32.Parse(res[0]);
                    repetition = Int32.Parse(res[1]);
                    for (int i = 0; i < _trackedBodyParts; i++)
                    {
                        _effortPerBodyPart[i] = float.Parse(res[i + 2]); // * _participantWeight * _weightPercentage[i];
                    }
                    results.Add(_effortPerBodyPart);
                    
                }
                catch
                {
                    UnityEngine.Debug.Log("Something wrong in the effortComputed file");
                }
            }

            return results;
        }
}
}
