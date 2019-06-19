using Mapping.Types;
using Mapping.Types.ChangeDetectors;
using System;
using Assets.Scripts.Mapping.Types.ChangeDetectors;
using UnityEngine;
using Mapping;
using System.Collections.Generic;

namespace Assets.Scripts.Mapping.Types
{
    public class LearnStaticMapping : MappingType
    {
        private ChangeDetector _changeDetector;
        private List<GameObject> _bones;

        public LearnStaticMapping()
        {
            _bones = new List<GameObject>();
        }

        public void Change(float amount)
        {
            foreach (var bone in _bones)
            {

                var newScale = bone.transform.localScale;
                newScale.y = IsChanging();

                if (newScale.y <= 1) return;

                // todo: add counter scale for the children bones
                bone.transform.localScale = newScale;
            }

        }

        public float IsChanging()
        {
            return _changeDetector.IsChanging();
        }

        public void SetBone(GameObject bone)
        {
            _bones.Add(bone);
        }

        public void SetChangeDetector(ChangeDetector changeDetector)
        {
            _changeDetector = changeDetector;
        }
    }
}
