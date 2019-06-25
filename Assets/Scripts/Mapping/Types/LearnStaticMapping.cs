using Mapping.Types;
using Mapping.Types.ChangeDetectors;
using System;
using Assets.Scripts.Mapping.Types.ChangeDetectors;
using UnityEngine;
using Mapping;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Mapping.Types
{
    public class LearnStaticMapping : MappingType
    {
        private ChangeDetector _changeDetector;
        private List<GameObject> _bones;
        private List<GameObject> _bonesChildren;

        public LearnStaticMapping()
        {
            _bones = new List<GameObject>();
            _bonesChildren = new List<GameObject>();
        }

        public void Change(float amount)
        {
            var newYScale = IsChanging();
            _bones.ForEach(b =>
            {
                var newScale = b.transform.localScale;
                newScale.y = newYScale;
                if (newScale.y <= 1) return;
                b.transform.localScale = newScale;
            } );
            _bonesChildren.ForEach(b =>
            {
                var newScale = b.transform.localScale;
                newScale.y = 1/newYScale;
                b.transform.localScale = newScale;
            });
        }

        public float IsChanging()
        {
            return _changeDetector.IsChanging();
        }

        public void SetBone(GameObject bone)
        {
            _bones.Add(bone);
            _bonesChildren.Add(bone.transform.GetComponentsInChildren<Transform>().First(t => t.CompareTag("Hand")).gameObject);
        }

        public void SetChangeDetector(ChangeDetector changeDetector)
        {
            _changeDetector = changeDetector;
        }
    }
}
