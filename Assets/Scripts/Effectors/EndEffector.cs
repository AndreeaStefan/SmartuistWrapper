using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Assessment;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Effectors
{
    public class EndEffector : MonoBehaviour
    {
        
        

        public AnhaActor actor;
        private Assessor _assessor;
        private Questionnaire _questionnaire;


        // Start is called before the first frame update
        void Start()
        {
            name = $"EndEffector_{transform.parent.name}";
            actor.AddEffector(this);

            _assessor = FindObjectsOfType<Assessor>()[0];
            _assessor.AddEffector(this);

            _questionnaire = FindObjectOfType<Questionnaire>();
        }

        public void Initialise(Assessor assessor)
        {
            _assessor = assessor;
        }
        

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.gameObject.CompareTag("Target"))
            {
                _assessor.StopRepetition();
            }

            if (other.transform.gameObject.CompareTag("Evaluation"))
            {
                _questionnaire.HitTarget(other.gameObject);
            }
        }

    }
}
