using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Assessment;
using Mapping.Types.ChangeDetectors;
using UnityEditor.UIElements;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Assets.Scripts.Mapping.Types.ChangeDetectors
{
    public class LearnStaticChange : ChangeDetector
    {
        private Assessor _assessor;
        private float _currentScale;

        private float _increaseScaleRate;
        private float _decreaseScaleRate;
        private float _learningRate;
        

        private float _previousResult;
        private bool _firstBatch;
        private float _previousDelta;
        private float _noSignChanges;
        private float _signOscillations;

        private StreamWriter writer; 

        public LearnStaticChange(Assessor assessor, float scale)
        {
            _assessor = assessor;
            _currentScale = scale;

            _increaseScaleRate = 0.5f;
            _decreaseScaleRate = 0.05f;

            _firstBatch = true;
            _noSignChanges = 0;
            _signOscillations = 0;
            _learningRate = 0.3f;

            var file = new FileStream(@"deltas.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            writer = new StreamWriter(file);
            writer.WriteLine("Start");
            writer.Flush();
        }

        public float IsChanging()
        {
            if (_assessor.targetsTapped == _assessor.BatchSize  )
            {
                _assessor.targetsTapped = 0;
                var nextScale = GetNextScale();
                UnityEngine.Debug.Log("Next Scale" + nextScale);
                return nextScale;
            }
            return _currentScale;         
        }

        // get the next scale based on some computational magic
        private float GetNextScale()
        {
            var nextScale = _currentScale;
            var results = _assessor.GetBatchResult();
            if (results.Count == _assessor.BatchSize)
            {
                var currentResult = GetCombinedResult(results);

                if (_firstBatch) // 
                {
                    _previousResult = currentResult;
                    _previousDelta = 0;
                    _firstBatch = false;
                }
                else
                {
                    var delta = (_previousResult - currentResult) / 2;
                    nextScale = _currentScale + _learningRate * Math.Sign(delta) * Normalize(Math.Abs(delta), 0,2000, 1, 3);
                    _previousResult = currentResult;
                    _currentScale = nextScale;
                    UnityEngine.Debug.Log("Delta" + delta + "Norm" + Normalize(Math.Abs(delta), 0,2000, 1, 3));
                    writer.WriteLine(delta);
                    writer.WriteLine("Scale:" + nextScale);
                    writer.Flush();
                    AdaptLearingRate(delta);
                }

                /*
                if (delta > 0)
                {
                    nextScale = _currentScale + _increaseScaleRate;
                    _previousResult = currentResult;
                    _currentScale = nextScale;
                }
                else
                {
                    nextScale = _currentScale - _decreaseScaleRate;
                    _previousResult = currentResult;
                    _currentScale = nextScale;
                }
                */
               
            }
           
            return nextScale;
        }

        private void AdaptLearingRate(float delta)
        {
            if (Math.Sign(_previousDelta) == delta)
            {
                _noSignChanges++;
            }
            else
            {
                _signOscillations++;
            }

            if (_noSignChanges >= 3) // increase learing rate 
            {
                _learningRate += 0.1f;
                UnityEngine.Debug.Log("Learning rate increased to: " + _learningRate);
                _noSignChanges = 0;
            }

            if (_signOscillations >= 3) // decrease learing rate 
            {
                _learningRate -= 0.01f;
                UnityEngine.Debug.Log("Learning rate decreased to: " + _learningRate);
                _signOscillations = 0;
            }

            _previousDelta = delta;
        }

        // TODO: a better way of combining the results + use all the effort 
        private float GetCombinedResult(List<RepetitionResult> results)
        {

            var sum = 0f;

            foreach (var res in results)
            {
                sum += res.MovementTime;
            }

            return sum / results.Count;
        }

        private float Normalize(float val, float valmin, float valmax, float min, float max)
        {
            return (((val - valmin) / (valmax - valmin)) * (max - min)) + min;
        }


    }
}

