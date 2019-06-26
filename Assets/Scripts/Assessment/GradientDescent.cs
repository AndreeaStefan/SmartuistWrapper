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
        private float _previousGain;
        private float _previousDelta;
        private float _noSignChanges;
        private float _signOscillations;
        private int _trackedBodyParts = 7;
        private float[] _previousResults;
        private int direction;

        public float Gain;
        public float Delta;
        private List<RepetitionResult> repResults;

        public GradientDescent(float scale, int batchSize)
        {
            _currentScale = scale;
            _firstBatch = true;
            _noSignChanges = 0;
            _signOscillations = 0;
            _learningRate = 0.3f;
            _batchSize = batchSize;
            Gain = -1f;
            direction = 1;
        }

        // get the next scale based on some computational magic
        public float GetNextScale(List<RepetitionResult> results)
        {
            var nextScale = _currentScale;
            repResults = results;
            var currentResults = GetCombinedResult(results);

            if (_firstBatch) // 
            {
                _previousResults = currentResults;
                _previousGain = 0;
                _firstBatch = false;
            }
            else
            {
                Gain = GetGain(currentResults);

                Delta = Gain - _previousGain;
                var normDelta = Normalize(Delta, 0, 10, 0, 3);

                direction = Delta < 0 ? -1 * direction : direction;
                nextScale = _currentScale + Math.Abs(_learningRate * Delta) * direction;
                UnityEngine.Debug.Log("Gain " + Gain + " delta:  " + Delta + " normDelta:  " + normDelta);
                _previousGain = Gain;
                _currentScale = nextScale;

                AdaptLearingRate(Delta);
            }

           

            return nextScale;
        }

        private float GetGain(float[] results)
        {
            var gainEffort = 0f;
            var gainThroughput = (float) Math.Abs(_previousResults[_trackedBodyParts] - results[_trackedBodyParts]);

            for (int i = 0; i < _trackedBodyParts; i++)
            {
                gainEffort += (float) Math.Abs(_previousResults[i] - results[i]); // based on sum of squard error
            }

            gainEffort =  gainEffort / (_trackedBodyParts) ;
            UnityEngine.Debug.Log(repResults[0].Lesson + " gainEffort " + gainEffort + " gainThroughput " + gainThroughput);
            return (gainEffort + 5 * gainThroughput) / 6;
        }

        private void AdaptLearingRate(float delta)
        {
            var newDir = Delta < 0 ? -1 * direction : direction;
            if (direction == newDir)
            {          
                _noSignChanges++;
            }
            else
            {
                _signOscillations++;
            }

            if (_noSignChanges > 1) // increase learing rate 
            {
                _learningRate *= 1.1f;
                UnityEngine.Debug.Log("Learning rate increased to: " + _learningRate);
                _noSignChanges = 0;
            }

            if (_signOscillations > 1) // decrease learing rate 
            {
                _learningRate *= 0.8f;
                UnityEngine.Debug.Log("Learning rate decreased to: " + _learningRate);
                _signOscillations = 0;
            }

        }

        private void DecreaseLearingRate(float delta)
        {

        }

        // Average across all the repetitions in the lesson - for each part of the result (throughput and effort per body part) 
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
                for (int i = 0; i < _trackedBodyParts; i++)
                {
                    sum[i] += res.DifficultyIndexNorm / effortPerBodyPart[i] ;
                }

                sumTh += (1000 * res.DifficultyIndexNorm) / res.MovementTime;
                sum[_trackedBodyParts] += sumTh;
                count++;

            }

            for (int i = 0; i <= _trackedBodyParts; i++)
            {
                sum[i] = sum[i] / count;
            }

            return sum;
        }

        private float Normalize(float val, float valmin, float valmax, float min, float max)
        {
            return (((val - valmin) / (valmax - valmin)) * (max - min)) + min;
        }
    }
}
