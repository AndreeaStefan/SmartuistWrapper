using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /***
     *** Deprecated 
     ***/
    public class PoseOffsets
    {
        private static readonly List<Quaternion> offsets = new List<Quaternion>
        {
            new Quaternion(-4.37112782E-08f,  2.38418608E-07f,  -1f,  -5.20548554E-07f),
            new Quaternion(1.34308857E-06f,  -9.69377538E-07f,  -2.38418323E-07f,  1f),
            new Quaternion(-1.3113023E-06f,  3.79321591E-07f,  2.38418721E-07f,  1f),
            new Quaternion(9.51351524E-07f,  -9.69377538E-07f,  -8.88463226E-14f,  1f),
            new Quaternion(-1.54972122E-06f,  1.26440511E-07f,  -2.16562455E-07f,  1f),
            new Quaternion(-6.32202784E-07f,  9.46281318E-07f,  1.14692352E-06f,  1f),
            new Quaternion(-5.05762102E-07f,  -1.57053739E-06f,  3.78310403E-07f,  -0.99999994f),
            new Quaternion(2.66236697E-07f,  4.37114771E-08f,  -1.54972076E-06f,  1f),
            new Quaternion(-1.01241984E-08f,  4.37118821E-08f,  -1.54972076E-06f,  1f),
            new Quaternion(-5.8475581E-07f,  4.37128165E-08f,  -1.70071667E-06f,  1f),
            new Quaternion(8.02690863E-07f,  4.37113989E-08f,  -1.22387951E-06f,  1f),
            new Quaternion(-0.0436195433f,  2.57593825E-07f,  1.68492056E-06f,  -0.999048233f),
            new Quaternion(-2.82130429E-07f,  -1.01238076E-08f,  -1.60158027E-06f,  0.99999994f),
            new Quaternion(2.83122034E-07f,  2.83122034E-07f,  1.04308121E-07f,  -0.99999994f),
            new Quaternion(-1.59442413E-06f,  5.8114523E-07f,  2.83122034E-07f,  0.99999994f),
            new Quaternion(2.98023195E-07f,  2.98023195E-07f,  6.85453358E-07f,  -0.99999994f),
            new Quaternion(-1.20699394E-06f,  5.51342907E-07f,  5.51342907E-07f,  0.99999994f),
            new Quaternion(2.38418565E-07f,  3.5762784E-07f,  9.23871937E-07f,  -0.99999994f),
            new Quaternion(2.83122034E-07f,  4.02331324E-07f,  6.40749874E-07f,  0.99999994f)
        };

        public static Quaternion GetOffsetForBone(HumanBodyBones bone)
        {
            return offsets[(int) bone];
        }
    }
}