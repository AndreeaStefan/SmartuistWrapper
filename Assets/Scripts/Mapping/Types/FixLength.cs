using System.Collections.Generic;
using System.Linq;
using Mapping.Types;
using Mapping.Types.ChangeDetectors;
using UnityEngine;

namespace Assets.Scripts.Mapping.Types
{
    public class FixLength: MappingType
    {

        private List<GameObject> _bones;
        private List<GameObject> _bonesChildren;
        private readonly float _scale;

        public FixLength(float scale)
        {
            _scale = scale;
            _bones = new List<GameObject>();
            _bonesChildren = new List<GameObject>();
        }

        public void Change(float how)
        {
            var newYScale = _scale;
            _bones.ForEach(b =>
            {
                var newScale = b.transform.localScale;
                newScale.y = newYScale;
                b.transform.localScale = newScale;
            });
            _bonesChildren.ForEach(b =>
            {
                var newScale = b.transform.localScale;
                newScale.y = 1 / newYScale;
                b.transform.localScale = newScale;
            });
        }

        public float IsChanging()
        {
            return 1;
        }
        public void SetChangeDetector(ChangeDetector changeDetector)
        {
        }

        public void SetBone(GameObject bone)
        {
            _bones.Add(bone);
            _bonesChildren.Add(bone.transform.GetComponentsInChildren<Transform>().First(t => t.CompareTag("Hand")).gameObject);
        }
    }
}
