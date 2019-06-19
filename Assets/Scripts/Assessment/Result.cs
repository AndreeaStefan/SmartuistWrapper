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
            var width = targetSize * 10; // the values should be > 1 
            var distance = Vector3.Distance(previousActorPosition.position, targetPosition); //idk if this is fine for depth or I should just use the target depth....

            var id = Method3_Cha();
            var id1 = Method1_Qian(); // paper from Jonas
            var id2 = Method2_Murata();

            var file = new FileStream(@"Difficulty.txt", FileMode.Append, FileAccess.Write, FileShare.Write);
            writer = new StreamWriter(file);
            writer.WriteLine(@" Target: " + targetIndex + " "+ width + " " + distance + "  --- Id: " + id + " Id1: " + id1 + " Id2: " + id2);
            writer.Flush();
            writer.Close();

            return id;
        }

        // DificultyIndex - using the method form the paper from Jonas --> The Eyes Don’t Have It: ...
        private float Method1_Qian()
        {
            // id = log((alpha / w) + 1) 
            // alpha - angular distance 
            // w - angular size of targets

            var width = targetSize * 10; // the values should be > 1 
            var distance = Vector3.Distance(previousActorPosition.position, targetPosition); //idk if this is fine for depth or I should just use the target depth....

            var alpha = Vector3.Angle(previousActorPosition.position, targetPosition); // angular distance  
            var omega = 2 * Math.Atan(width / distance); // angular size of target  

            var id = Math.Log(alpha / omega + 1);
            return (float) id;
        }

        // DificultyIndex - Extending Fitts’ law to a three-dimensional pointing task
        private float Method2_Murata()
        {
            // id = log((dist / w) + 1) + c sin(theta) 

            var width = targetSize * 10; // the values should be > 1 
            var distance = Vector3.Distance(previousActorPosition.position, targetPosition); 
            var theta = Vector3.Angle(previousActorPosition.position, targetPosition); // they use a different angle here 

            var id =  Math.Log((distance / width) + 1) + Math.Sin(theta);
            return (float) id;
        }
        // Extended Fitts’ law for 3D pointing tasks using 3D target arrangements
        private float Method3_Cha()
        {
            // movement time = a + b theta1 + c sin(theta2) + d log(2d/w) - from: Extended Fitts’ law for 3D pointing tasks..... Yeonjoo Cha and Rohae Myung 
            // theta1 - inclination angle: angle between the positive z-axis and the target location
            // theta2 - azimuth angle: angle between the positive x-axis and the projected target location on the xy plane

            var width = targetSize * 10; // the values should be > 1 
            var distance = Vector3.Distance(previousActorPosition.position, targetPosition); //idk if this is fine for depth or I should just use the target depth....

            var theta1 = Vector3.Angle(previousActorPosition.position, targetPosition);

            var direction = (previousActorPosition.position - targetPosition).normalized; //  previousActorPosition.InverseTransformDirection( ...) 
            var theta2 = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

            var projectionOnZActor = Vector3.ProjectOnPlane(previousActorPosition.position, Vector3.forward);
            var projectionOnZTarget = Vector3.ProjectOnPlane(targetPosition, Vector3.forward);
            var theta2_proj = Vector3.Angle(projectionOnZActor, projectionOnZTarget);

            var id = theta1 + Mathf.Sin(theta2_proj) + Mathf.Log(2 * distance / width);

            return (float) id;
        }
    }
}