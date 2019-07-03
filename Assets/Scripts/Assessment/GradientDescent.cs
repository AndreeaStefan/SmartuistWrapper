using Assessment;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly string deltasFileName = "GradientStats.csv";
        private StreamWriter statsSW;

        public float Gain;
        public float Delta;
        private List<RepetitionResult> repResults;

        public GradientDescent(float scale, int batchSize)
        {
            _currentScale = scale;
            _firstBatch = true;
            _noSignChanges = 0;
            _signOscillations = 0;
            _learningRate = 0.2f;
            _batchSize = batchSize;
            Gain = -1f;
            direction = 1;

            statsSW = new StreamWriter(deltasFileName, true);
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
                Delta = Delta > -1.6 ? Math.Abs(Delta) : Delta;
                direction = Delta < 0 ? -1 * direction : direction;
                nextScale = _currentScale + _learningRate *  Gain * Math.Sign(Delta);
                UnityEngine.Debug.Log("Gain " + Gain + " delta:  " + Delta);
                _previousGain = Gain;
                _currentScale = nextScale;

                AdaptLearingRateWithGain();
            }

           

            return nextScale;
        }

        private float GetGain(float[] results)
        {
            var gainEffort = 0f;
            var gainThroughput = (float)Math.Sqrt(results[_trackedBodyParts]); 
            for (int i = 0; i < _trackedBodyParts; i++)
            {
                gainEffort += (float )Math.Sqrt( results[i]);
            }

            gainEffort =  gainEffort / (_trackedBodyParts) ;

            var gain = (gainEffort + 2 * gainThroughput) / 3;
            statsSW.WriteLine($"{repResults[0].Player}, {repResults[0].Lesson},{gainEffort },{gainThroughput},{_previousGain},{gain}, {_learningRate}, {results[_trackedBodyParts]}");
            statsSW.Flush();
            UnityEngine.Debug.Log(repResults[0].Lesson + " gainEffort " + gainEffort + " gainThroughput " + gainThroughput);
            return gain;
        }

        private void AdaptLearingRate()
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


        private void AdaptLearingRateWithGain()
        {
           
            if (Delta > 0) // increase learing rate 
            {
                _noSignChanges++;
            }
            else
            {
                _noSignChanges = 0;
                _learningRate *= 0.85f;
                UnityEngine.Debug.Log("Learning rate decreased to: " + _learningRate);
            }

            if (_noSignChanges > 3)
            {
                _learningRate *= 1.1f;
                _noSignChanges = 0;
                UnityEngine.Debug.Log("Learning rate increased to: " + _learningRate);
            }
            
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
