using System;
using System.IO;
using UnityEngine;

namespace Assessment
{
    public class Result
    {
        public int targetIndex;
        public float targetSize;
        public float targetDepth;
        public float movementTime;
        public string mappingType;
        public float difficultyIndex;

        public Transform previousActorPosition;
        public Vector3 targetPosition;

        private StreamWriter writer;

        public Result(int targetIndex, float targetSize, float targetDepth, float time, string mappping, 
            Transform actorPos,
            Vector3 targetPos)
        {
            this.targetIndex = targetIndex;
            this.targetDepth = targetDepth;
            this.targetSize = targetSize;
            movementTime = time;
            mappingType = mappping;
            previousActorPosition = actorPos;
            targetPosition = targetPos;

            ComputeDifficultyIndex();

        }

        private float ComputeDifficultyIndex()
        {
            // movement time = a + b theta1 + c sin(theta2) + d log(2d/w) - from: Extended Fitts’ law for 3D pointing tasks..... Yeonjoo Cha and Rohae Myung 
            // theta1 - inclination angle: angle between the positive z-axis and the target location
            // theta2 - azimuth angle: angle between the positive x-axis and the projected target location on the xy plane

            var width = targetSize * 10; // the values should be > 1 
            var distance = Vector3.Distance(previousActorPosition.position, targetPosition); //idk if this is fine for depth or I should just use the target depth....

            var theta1 = Vector3.Angle(previousActorPosition.position, targetPosition);

            var direction  = (previousActorPosition.position - targetPosition).normalized; //  previousActorPosition.InverseTransformDirection( ...) 
            var theta2 = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

            var projectionOnZActor = Vector3.ProjectOnPlane(previousActorPosition.position, Vector3.forward);
            var projectionOnZTarget = Vector3.ProjectOnPlane(targetPosition, Vector3.forward);
            var theta2_proj = Vector3.Angle(projectionOnZActor, projectionOnZTarget);

            var id = theta1 + Mathf.Sin(theta2_proj) + Mathf.Log(2 * distance / width);
            // id = log((alpha / w) + 1) from: The Eyes Don’t Have It...
            // alpha - rotation angle between 2 targets
            // w - angular size of targets

            var amplitude = targetPosition.y; // using this does not make sense 
            // Vector3.Angle(previousActorPosition.position, targetPosition); // previousActorPosition - the position of the actor when it hit the prev. target  
            var alpha = 2 * Math.Atan(0.5 * amplitude / distance); // angular amplitude 
            var omega = 2 * Math.Atan( width / distance); // angular size of target --- 

            var id1 = Math.Log( alpha / omega + 1); // paper from Jonas

            // id = log((dist / w) + 1) + c sin(theta) from: Extending Fitts’ law to a three-dimensional pointing task
            var id2 = Math.Log((distance / width) + 1) + Math.Sin(theta1);

            var file = new FileStream(@"Difficulty.txt", FileMode.Append, FileAccess.Write, FileShare.Write);
            writer = new StreamWriter(file);
            writer.WriteLine(@" Target: " + width + " " + distance + "  --- Id: " + id + " Id1: " + id1 + " Id2: " + id2);
            writer.Flush();
            writer.Close();


            return id;
        }
    }
}