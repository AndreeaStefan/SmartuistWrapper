using Assets.Scripts.Mapping;
using Assets.Scripts.Utils;
using UnityEngine;

public class Pose
    {
        public bool stored;
        public Vector3 hip;
        public Vector3 forward;
        public Quaternion[] rotation;
        
        public int Length
        {
            get
            {
                if (rotation != null)
                    return rotation.Length;
                return 0;
            }
            set => rotation = new Quaternion[value];
        }

        public Vector3 Left => Vector3.Cross(forward, Vector3.up);

        public void Store(BasicBoneMapping bonesComponent)
        {
            var bones = bonesComponent.Bones();
            var hips = bones[bonesComponent.RootBone()].transform;
            var hipsParent = hips.parent;
            
            Length = bones.Count;
            hip = hips.localPosition;
            forward = !(hipsParent == null) ? hipsParent.forward : hips.forward;
            forward.y = 0.0f;
            forward.Normalize();
            
            for (var index = 0; index < Length; ++index)
            {
                if (bones[index] != null)
                    rotation[index] = bones[index].transform.rotation;
            }
            stored = true;
        }
        
        public Vector3 EstimateForwardFrom(Transform[] bones)
        {
            if (!stored || bones.Length != Length)
                return forward;
            Vector3 zero = Vector3.zero;
            int num = 0;
            for (int index = 0; index < Length; ++index)
            {
                if (bones[index] != null)
                {
                    zero += bones[index].rotation * Quaternion.Inverse(rotation[index]) * forward;
                    ++num;
                }
            }
            return zero / num;
        }

        public void ApplyPoseTo(Transform[] bones)
        {
            if (Length != bones.Length)
                return;
            bones[0].localPosition = hip;
            for (int index = 0; index < Length; ++index)
            {
                if (bones[index] != null)
                    bones[index].rotation = rotation[index];
            }
        }

        // todo: change when different number of bones
        public Quaternion[] ExtractRotationOffsets()
        {
            var referencePose = ReferencePose();
            var quaternionArray = new Quaternion[Mathf.Min(referencePose.rotation.Length, rotation.Length)];
            var quaternion1 = Quaternion.Inverse(Quaternion.LookRotation(referencePose.forward));
            var quaternion2 = Quaternion.Inverse(Quaternion.LookRotation(forward));
            for (var index = 0; index < quaternionArray.Length; ++index)
                quaternionArray[index] = Quaternion.Inverse(quaternion1 * referencePose.rotation[index]) * quaternion2 * rotation[index];
            return quaternionArray;
        }


        private static Pose ReferencePose ()
        {
            var bodyPose = new Pose {Length = 19};
            var quaternion1 = Quaternion.Euler(0.0f, 0.0f, 180f);
            var quaternion2 = Quaternion.Euler(90f, 0.0f, -90f);
            var quaternion3 = Quaternion.Euler(90f, 0.0f, 90f);
            var quaternion4 = Quaternion.Euler(0.0f, 90f, 0.0f);
            var quaternion5 = Quaternion.Euler(0.0f, -90f, 0.0f);
            var quaternion6 = Quaternion.Euler(90f, 180f, 0.0f);

            // shoulders
            bodyPose.rotation[11] = Quaternion.Euler(0.0f, 0.0f, -90f);
            bodyPose.rotation[12] = Quaternion.Euler(0.0f, 0.0f, 90f);

            // main body - head, spine, chest, neck, head
            bodyPose.rotation[(int) Bones.Hips] = quaternion1;
            bodyPose.rotation[7] = quaternion1;
            bodyPose.rotation[8] = quaternion1;
            bodyPose.rotation[9] = quaternion1;
            bodyPose.rotation[10] = quaternion1;

            // left arm (upper, lower, hand)
            bodyPose.rotation[13] = quaternion2;
            bodyPose.rotation[15] = quaternion2;
            bodyPose.rotation[17] = quaternion2;

            // right arm (upper, lower, hand)
            bodyPose.rotation[14] = quaternion3;
            bodyPose.rotation[16] = quaternion3;
            bodyPose.rotation[18] = quaternion3;

            // left leg 
            bodyPose.rotation[(int)Bones.LeftUpperLeg] = quaternion4;
            bodyPose.rotation[(int)Bones.LeftLowerLeg] = quaternion4;

            // right leg 
            bodyPose.rotation[2] = quaternion5;
            bodyPose.rotation[4] = quaternion5;

            // feet
            bodyPose.rotation[6] = quaternion6;
            bodyPose.rotation[5] = quaternion6;

        bodyPose.forward = Vector3.forward;
            return bodyPose;
        }
        
        
    }
