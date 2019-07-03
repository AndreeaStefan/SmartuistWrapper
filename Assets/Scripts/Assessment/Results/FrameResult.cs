using System;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Utils;
using Effectors;
using Rokoko.Smartsuit;
using UnityEngine;

namespace Assessment
{
    public class FrameResult
    {
        public string PlayerName;
        public int Lesson;
        public int Repetition;
        public float DistanceFromMain;
        public float DistanceFromLeftE;
        public float DistanceFromRightE;
        public Vector3 PositionMain;
        public Vector3 PositionLeftHand;
        public Vector3 PositionLeftArm;
        public Vector3 PositionRightHand;
        public Vector3 PositionRightArm;
        public Vector3 PositionLeftFoot;
        public Vector3 PositionRightFoot;
        public DateTime Time;


        public static FrameResult GetNewFrame(AnhaActor actor, Vector3 targetPosition, EndEffector eL, EndEffector eR,
            string player, int lesson, int repetition)
        {
            var positions = actor.Bones.Select(s => s.transform.position).ToList();
            return new FrameResult
            {
                PlayerName = player,
                Lesson = lesson,
                Repetition = repetition,
                DistanceFromMain = Vector3.Distance(actor.camera.transform.position, targetPosition),
                DistanceFromLeftE = Vector3.Distance(eL.transform.position, targetPosition),
                DistanceFromRightE = Vector3.Distance(eR.transform.position, targetPosition),
                PositionMain = actor.camera.transform.position,
                PositionLeftArm = positions[(int) Bones.LeftUpperArm],
                PositionRightArm = positions[(int) Bones.RightUpperArm],
                PositionRightFoot = positions[(int) Bones.RightFoot],
                PositionLeftFoot = positions[(int) Bones.LeftFoot],
                PositionLeftHand = positions[(int) Bones.LeftHand],
                PositionRightHand = positions[(int) Bones.RightHand],
                Time = DateTime.Now
            };
        }


        public override string ToString()
        {
            // total size: 28 entries. 6 - 26 - vectors. 27 - time
            return
                $"{PlayerName},{Lesson},{Repetition},{DistanceFromMain},{DistanceFromLeftE},{DistanceFromRightE},{V3ToS(PositionMain)},{V3ToS(PositionLeftHand)},{V3ToS(PositionLeftArm)},{V3ToS(PositionRightHand)},{V3ToS(PositionRightArm)},{V3ToS(PositionLeftFoot)},{V3ToS(PositionRightFoot)},{Time.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
        }

        private string V3ToS(Vector3 v)
        {
            return $"{v.x:F1},{v.y:F1},{v.z:F1}";
        }
    }
}