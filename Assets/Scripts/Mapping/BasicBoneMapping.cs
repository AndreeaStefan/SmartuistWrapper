using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Mapping
{
    public class BasicBoneMapping : MonoBehaviour
    {
        [HideInInspector]
        public List<GameObject> _bones;
        public GameObject Hips;
        public GameObject Head;


        private void Awake()
        {
            _bones = new List<GameObject>
            {
                Hips
            };
        }

        public virtual List<GameObject> Bones()
        {
            return _bones;
        }

        public virtual int RootBone()
        {
            return 0;
        }
    }
}
