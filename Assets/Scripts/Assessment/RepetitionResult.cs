
using UnityEngine;

namespace Assessment
{
    public class RepetitionResult
    {
        public string Player;
        public int Lesson;
        public int Repetition;
        public int TargetIndex;
        public Vector3 TargetSize;
        public Vector3 TargetPosition;
        public long MovementTime;

        public RepetitionResult(string player, int lesson, int repetition, int targetIndex, Vector3 targetSize, Vector3 targetPosition, long time)
        {
            Player = player;
            Lesson = lesson;
            Repetition = repetition;
            TargetIndex = targetIndex;
            TargetPosition = targetPosition;
            TargetSize = targetSize;
            MovementTime = time;
        }

        public override string ToString()
        {
            return $"{Player},{Lesson},{Repetition},{TargetIndex},{TargetSize},{TargetPosition},{MovementTime}";
        }
    }
}