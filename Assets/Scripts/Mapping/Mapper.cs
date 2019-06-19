using System.Linq;
using Assessment;
using Assets.Scripts.Mapping.Types.ChangeDetectors;
using Assets.Scripts.Utils;
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
            var assessor = FindObjectsOfType<Assessor>()[0];
            var changeDetector2 = new LearnStaticChange(actor.BatchSize, assessor, gameObject.transform.localScale.y);

            switch (actor.academy.Mapping)
            {
                case Mappings.SteadyPointing:

                    var changeDetector = new SteadyPointing(gameObject);
                    mapping.SetBone(gameObject);
                    mapping.SetChangeDetector(changeDetector);
                    break;

                case Mappings.WristMove:
                    var hand = Helper.ChildWithTag(gameObject.transform, "Hand");
                    var changeDetector1 = new WristMove(hand);
                    mapping.SetBone(gameObject);
                    mapping.SetChangeDetector(changeDetector1);
                    break;

                case Mappings.LearnStaticMapping:
                    var mappers = FindObjectsOfType<Mapper>();
                    foreach (var m in mappers)
                    {
                       mapping.SetBone(m.gameObject);
                       mapping.SetChangeDetector(changeDetector2); 
                    }
                    
                    break;
            }
        }
    }
}