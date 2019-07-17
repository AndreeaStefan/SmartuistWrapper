using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapping.Types;
using Mapping.Types.ChangeDetectors;
using UnityEngine;

namespace Assets.Scripts.Mapping.Types
{
    public class FixLength: MappingType
    {

        private GameObject _bone;
        private readonly float _scale;

        public FixLength(float scale)
        {
            _scale = scale;
        }

        public void Change(float how)
        {
            _bone.transform.localScale = new Vector3(_scale, _scale, _scale);
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
            _bone = bone;
        }
    }
}
