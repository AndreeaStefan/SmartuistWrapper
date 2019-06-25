
using System;
using System.IO;
using System.Numerics;
using Assets.Scripts.Assessment;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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
        public Vector3 ActorPosition;
        public long MovementTime;
        public float DifficultyIndex;
        public float TargetAngle;
        public float Throughput;
        public EffortResult EffortResult;

        public RepetitionResult(string player, int lesson, int repetition, int targetIndex,
            Vector3 targetSize, Vector3 targetPosition, Vector3 actorPosition,
            float angle, long time, EffortResult effortResult)
        {
            Player = player;
            Lesson = lesson;
            Repetition = repetition;
            TargetIndex = targetIndex;
            TargetPosition = targetPosition;
            TargetSize = targetSize;
            MovementTime = time;
            ActorPosition = actorPosition;
            DifficultyIndex = ComputeDifficultyIndex();
            if(MovementTime != 0)
                Throughput = 1000 * DifficultyIndex / MovementTime;
            TargetAngle = angle;
            EffortResult = effortResult;
        }

        public override string ToString()
        {
            return $"{Player},{Lesson},{Repetition},{TargetIndex},{TargetSize},{TargetPosition},{MovementTime},{DifficultyIndex},{Throughput},{EffortResult}";
        }

        private float ComputeDifficultyIndex()
        {
            var width = TargetSize * 10; // the values should be > 1 
            var distance = Vector3.Distance(ActorPosition, TargetPosition); //idk if this is fine for depth or I should just use the target depth....

            var id = Method3_Cha();
            var id1 = Method1_Qian(); // paper from Jonas
            var id2 = Method2_Murata();

            return id1;
        }

        // DificultyIndex - using the method form the paper from Jonas --> The Eyes Don�t Have It: ...
        private float Method1_Qian()
        {
            // id = log((alpha / w) + 1) 
            // alpha - angular distance 
            // w - angular size of targets

            var width = TargetSize.y * 10; // the values should be > 1 
            var distance = Vector3.Distance(ActorPosition, TargetPosition); //idk if this is fine for depth or I should just use the target depth....

            var alpha = Vector3.Angle(ActorPosition, TargetPosition); // angular distance  
            var omega = 2 * Math.Atan(width / distance); // angular size of target  

            var id = Math.Log(alpha / omega + 1);
            return (float)id;
        }

        // DificultyIndex - Extending Fitts� law to a three-dimensional pointing task
        private float Method2_Murata()
        {
            // id = log((dist / w) + 1) + c sin(theta) 

            var width = TargetSize.y * 10; // the values should be > 1 
            var distance = Vector3.Distance(ActorPosition, TargetPosition);
            var theta = TargetAngle; 
            var id = Math.Log((distance / width) + 1) + Math.Sin(theta);
            return (float)id;
        }
        // Extended Fitts� law for 3D pointing tasks using 3D target arrangements
        private float Method3_Cha()
        {
            // movement time = a + b theta1 + c sin(theta2) + d log(2d/w) - from: Extended Fitts� law for 3D pointing tasks..... Yeonjoo Cha and Rohae Myung 
            // theta1 - inclination angle: angle between the positive z-axis and the target location
            // theta2 - azimuth angle: angle between the positive x-axis and the projected target location on the xy plane

            var width = TargetSize.y * 10; // the values should be > 1 
            var distance = Vector3.Distance(ActorPosition, TargetPosition); //idk if this is fine for depth or I should just use the target depth....

            var theta1 = Vector3.Angle(ActorPosition, TargetPosition);

            var direction = (ActorPosition - TargetPosition).normalized; //  previousActorPosition.InverseTransformDirection( ...) 
            var theta2 = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

            var projectionOnZActor = Vector3.ProjectOnPlane(ActorPosition, Vector3.forward);
            var projectionOnZTarget = Vector3.ProjectOnPlane(TargetPosition, Vector3.forward);
            var theta2_proj = Vector3.Angle(projectionOnZActor, projectionOnZTarget);

            var id = theta1 + Mathf.Sin(theta2_proj) + Mathf.Log(2 * distance / width);

            return (float)id;
        }
    }
}