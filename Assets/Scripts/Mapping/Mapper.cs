using Effectors;
using Mapping.Types;
using Mapping.Types.ChangeDetectors;
using UnityEngine;

namespace Mapping
{
    public class Mapper : MonoBehaviour
    {
        public AnhaActor actor;        
        private MappingType Mapping;

        private void Start()
        {
            actor.SetMapper(this);
        }

        private void Update()
        {
            var change = Mapping.IsChanging();
            if (change  == 0) return;
            Mapping.Change(change);
        }


        public void SetMapping(MappingType mapping)
        {
            Mapping = mapping;
            mapping.SetBone(gameObject);
            var endEffector = transform.GetComponentInChildren<EndEffector>();
            var changeDetector = new SteadyPointing(endEffector);
            mapping.SetChangeDetector(changeDetector);
        }
    }
}