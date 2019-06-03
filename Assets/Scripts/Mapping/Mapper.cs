using Assets.Scripts.Mapping.Types.ChangeDetectors;
using Effectors;
using Mapping.Types;
using Mapping.Types.ChangeDetectors;
using UnityEngine;

namespace Mapping
{
    public class Mapper : MonoBehaviour
    {
        public AnhaActor actor;
        public GameObject toExtend;
        private MappingType Mapping;


        private void Start()
        {
            if (toExtend == null)
            {
                toExtend = gameObject;
            }

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
            mapping.SetBone(toExtend);
            var changeDetector = new WristMove(gameObject, toExtend);//new SteadyPointing(gameObject);
            mapping.SetChangeDetector(changeDetector);
        }
    }
}