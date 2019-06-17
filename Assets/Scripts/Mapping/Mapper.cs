using System.Linq;
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
                
                case Mappings.Genetic:
                    var secondBone = GameObject.FindGameObjectsWithTag("Mapper").First(g => g != gameObject);
                    ((Genetic)mapping).SetSecondBone(secondBone);
                    mapping.SetBone(gameObject);
                    mapping.SetChangeDetector(null);
                    break;
            }
        }
    }
}