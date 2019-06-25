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

        private float _batchSize;
        private bool _firstBatch;
        private float _previousLoss;
        private float _noSignChanges;
        private float _signOscillations;
        private int _trackedBodyParts = 7;
        private float[] _previousResults;

        public float Loss; 

        public GradientDescent(float scale, int batchSize)
        {
            _currentScale = scale;
            _firstBatch = true;
            _noSignChanges = 0;
            _signOscillations = 0;
            _learningRate = 0.5f;
            _batchSize = batchSize;
            Loss = -1f;
        }

        // get the next scale based on some computational magic
        public float GetNextScale(List<RepetitionResult> results)
        {
            var nextScale = _currentScale;        
            var currentResults = GetCombinedResult(results);

            if (_firstBatch) // 
            {
                _previousResults = currentResults;
                _previousLoss = 0;
                _firstBatch = false;
            }
            else
            {
                Loss = GetLoss(currentResults);
                nextScale = _currentScale + _learningRate * Normalize(Loss,0, 50, 1, 3);
                UnityEngine.Debug.Log("Loss " + Loss + " _previousResult:  " + _previousLoss);
                _previousLoss = Loss;
                _currentScale = nextScale;

                AdaptLearingRate(Loss);
            }

           

            return nextScale;
        }

        private float GetLoss(float[] results)
        {
            var loss = 0f;
            for (int i = 0; i <= _trackedBodyParts; i++)
            {
                loss += (float) Math.Pow((_previousResults[i] - results[i]), 2); // based on sum of squard error
            }

            return loss / (_trackedBodyParts +1 ) ; 
        }

        private void AdaptLearingRate(float delta)
        {
            if (Math.Sign(_previousLoss) == delta)
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

            _previousLoss = delta;
        }

        // TODO: a better way of combining the results 
        private float[] GetCombinedResult(List<RepetitionResult> results)
        {

            var sumTh = 0f;
            var count = 0;
            float[] sum = new float[_trackedBodyParts + 1];
            

            for (int i = 0; i <= _trackedBodyParts; i++)
            {
                sum[i] = 0;
            }

            foreach (var res in results)
            {
                var effortPerBodyPart = res.EffortResult.GetEffortPerBodyPart();
                if (res.MovementTime != 0)
                {
                    sumTh += (1000 * res.DifficultyIndex) / res.MovementTime;
                    count++;
                }

                for (int i = 0; i < _trackedBodyParts; i++)
                {
                    sum[i] += effortPerBodyPart[i] / res.DifficultyIndex;
                }

                sum[_trackedBodyParts] += sumTh;

            }

            for (int i = 0; i <= _trackedBodyParts; i++)
            {
                sum[i] = sum[i] / count;
            }

            UnityEngine.Debug.Log("Combined results - Th: " + sumTh + " "  );
            return sum;
        }

        private float Normalize(float val, float valmin, float valmax, float min, float max)
        {
            return (((val - valmin) / (valmax - valmin)) * (max - min)) + min;
        }
    }
}
