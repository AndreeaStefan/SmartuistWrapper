using Assessment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Assessment
{
    public class GradientDescent
    {
        private float _currentScale;
        private float _learningRate;

        private float _previousResult;
        private float _batchSize;
        private bool _firstBatch;
        private float _previousDelta;
        private float _noSignChanges;
        private float _signOscillations;

        public float Delta; 

        public GradientDescent(float scale, int batchSize)
        {
            _currentScale = scale;
            _firstBatch = true;
            _noSignChanges = 0;
            _signOscillations = 0;
            _learningRate = 0.5f;
            _batchSize = batchSize;
            Delta = -1f;
        }

        // get the next scale based on some computational magic
        public float GetNextScale(List<RepetitionResult> results)
        {
            var nextScale = _currentScale;
            
            var currentResult = GetCombinedResult(results);
            if (currentResult != Single.PositiveInfinity || currentResult != Single.NegativeInfinity)
            {
                if (_firstBatch) // 
                {
                    _previousResult = currentResult;
                    _previousDelta = 0;
                    _firstBatch = false;
                }
                else
                {
                    Delta = (currentResult - _previousResult) ; // throughput should increase
                    nextScale = _currentScale + _learningRate * Delta;
                    UnityEngine.Debug.Log("Delta " + Delta + " currentResult:  " + currentResult +
                                          " _previousResult:  " + _previousResult);
                    _previousResult = currentResult;
                    _currentScale = nextScale;

                    AdaptLearingRate(Delta);
                }

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
            var sumTh = 0f;
            var count = 0;

            foreach (var res in results)
            {
                sum += res.MovementTime;
                if (res.MovementTime != 0)
                {
                    sumTh += (1000 * res.DifficultyIndex) / res.MovementTime;
                    count++;
                }
                   
            }
            UnityEngine.Debug.Log("Combined results: " + sumTh + " "  );
            return (sumTh / count);
        }

        private float Normalize(float val, float valmin, float valmax, float min, float max)
        {
            return (((val - valmin) / (valmax - valmin)) * (max - min)) + min;
        }
    }
}
