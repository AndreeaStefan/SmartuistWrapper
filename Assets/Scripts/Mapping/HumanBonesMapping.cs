using System.Collections.Generic;
using Assets.Scripts.Mapping;
using UnityEngine;

namespace Mapping
{
    public class HumanBonesMapping :  BasicBoneMapping
    {
        public GameObject LeftUpperLeg; 
        public GameObject RightUpperLeg;
        public GameObject LeftLowerLeg;
        public GameObject RightLowerLeg;
        public GameObject LeftFoot;
        public GameObject RightFoot;
        public GameObject Spine;
        public GameObject Chest;
        public GameObject Neck;
        public GameObject LeftShoulder;
        public GameObject RightShoulder;
        public GameObject LeftUpperArm;
        public GameObject RightUpperArm;
        public GameObject LeftLowerArm;
        public GameObject RightLowerArm;
        public GameObject LeftHand;
        public GameObject RightHand;

        private void Awake()
        {
            _bones = new List<GameObject>
            {
                Hips,
                LeftUpperLeg,
                RightUpperLeg,
                LeftLowerLeg,
                RightLowerLeg,
                LeftFoot,
                RightFoot,
                Spine,
                Chest,
                Neck,
                Head,
                LeftShoulder,
                RightShoulder,
                LeftUpperArm,
                RightUpperArm,
                LeftLowerArm,
                RightLowerArm,
                LeftHand,
                RightHand
            };
        }

        /// <summary>Get the indices of the bones that are assigned a value </summary>
        public List<int> GetBonesIndices()
        {
            var indices = new List<int>();
            for (var i = 0; i < _bones.Count; i++)
            {
                if (_bones[i] != null)
                {
                    indices.Add(i);
                }
            }

            return indices;
        }

        public override List<GameObject> Bones()
        {
            return _bones;
        }

        public override int RootBone()
        {
            return 0;
        }
    }
}