using System;
using Assessment;
using UnityEngine;

namespace Effectors
{
    public class EndEffector : MonoBehaviour
    {

        public AnhaActor actor;
        private Assessor _assessor;
        [HideInInspector] public new string name;

        // Start is called before the first frame update
        void Start()
        {
            _assessor = actor.assessor;
            name = $"EndEffector_{transform.parent.name}";
            _assessor.RegisterEffector(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log( $"{name} triggered on Enter at: {DateTime.Now}");
        }
    }
}
