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
        private float _positiveDelta;
        private float _negativeDelta;
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
            _positiveDelta = 0;
            _negativeDelta = 0;
            _learningRate = 0.13f;
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
                Delta = _currentScale > 5f
                    ? Delta < 0.15 ? -Math.Abs(Delta) : Delta
                    : Delta > -0.15 ? Math.Abs(Delta) : Delta;
                Delta = _negativeDelta > 1 ? Math.Abs(Delta) : Delta;
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
            var gainThroughput = (float)Math.Abs(results[_trackedBodyParts]); 
            for (int i = 0; i < _trackedBodyParts; i++)
            {
                gainEffort += (float )Math.Abs( results[i]);
            }

            gainEffort =  gainEffort / (_trackedBodyParts) ;

            var gain = (gainEffort + gainThroughput) / 2;
            statsSW.WriteLine($"{repResults[0].Player}, {repResults[0].Lesson},{gainEffort },{gainThroughput},{_previousGain},{gain}, {_learningRate}, {results[_trackedBodyParts]}");
            statsSW.Flush();
            UnityEngine.Debug.Log(repResults[0].Lesson + " gainEffort " + gainEffort + " gainThroughput " + gainThroughput);
            return gain;
        }


        private void AdaptLearingRateWithGain()
        {
           
            if (Delta > 0) // increase learing rate 
            {
                _positiveDelta++;
                _negativeDelta = 0;
            }
            else
            {
                _negativeDelta++;
                _positiveDelta = 0;
                _learningRate *= 0.85f;
                UnityEngine.Debug.Log("Learning rate decreased to: " + _learningRate);
            }

            if (_positiveDelta > 1)
            {

                _learningRate *= 1.1f;
                _positiveDelta = 0;
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
                    sum[i] += 1 / effortPerBodyPart[i] ;
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
