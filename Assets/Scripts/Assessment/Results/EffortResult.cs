using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Assessment.Results;
using UnityEngine;

namespace Assets.Scripts.Assessment
{
    public class EffortResult
    {
        private static readonly int _trackedBodyParts = 7;
        private static float _participantWeight;
        private static bool _isMale;

        private float[] _distancePerBodyPart;
        private float[] _effortPerBodyPart;

        // body, leftUpperArm, RightUpperArm, RightFoot, LeftFoot, LeftHand, RightHand
        public static float[] _weightPercentage;

        public static void SetStats(float weight, bool isMale)
        {
            _participantWeight = weight;
            _weightPercentage = isMale
                ? new[] {0.551f, 0.0325f, 0.0325f, 0.1668f, 0.1668f, 0.057f, 0.057f}
                : new[] {0.532f, 0.029f, 0.029f, 0.1843f, 0.1843f, 0.0497f, 0.0497f};
        }

        public static EffortResult GetNewResult()
        {
            var effort = new EffortResult();

            var frames = FrameResultDb.GetCurrent();
            FrameResultDb.NewRep();

            effort._distancePerBodyPart = frames.Select((f, i) =>
            {
                if (i > 0)
                    return new[]
                    {
                        Vector3.Distance(frames[i - 1].PositionMain, f.PositionMain),
                        Vector3.Distance(frames[i - 1].PositionLeftArm, f.PositionLeftArm),
                        Vector3.Distance(frames[i - 1].PositionRightArm, f.PositionRightArm),
                        Vector3.Distance(frames[i - 1].PositionRightFoot, f.PositionRightFoot),
                        Vector3.Distance(frames[i - 1].PositionLeftFoot, f.PositionLeftFoot),
                        Vector3.Distance(frames[i - 1].PositionLeftHand, f.PositionLeftHand),
                        Vector3.Distance(frames[i - 1].PositionRightHand, f.PositionRightHand)
                    };
                return new float[_trackedBodyParts];
            }).Aggregate(new float[_trackedBodyParts],
                (distances, next) => distances.Select((curr, i) => curr + next[i]).ToArray()).ToArray();

            effort._effortPerBodyPart = effort._distancePerBodyPart
                .Select((d, i) => d * _weightPercentage[i] * 100).ToArray();

            return effort;
        }

        public float[] GetEffortPerBodyPart()
        {
            return _effortPerBodyPart;
        }

        public override string ToString()
        {
            return $"{string.Join(",",_distancePerBodyPart)},{string.Join(",", _effortPerBodyPart)}";
        }
    }
}