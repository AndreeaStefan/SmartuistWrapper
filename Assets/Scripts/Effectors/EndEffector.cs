using Assessment;
using UnityEngine;

namespace Effectors
{
    public class EndEffector : MonoBehaviour
    {
        
        

        public AnhaActor actor;
        private Assessor _assessor;

        // Start is called before the first frame update
        void Start()
        {
            name = $"EndEffector_{transform.parent.name}";
            actor.AddEffector(this);

            _assessor = FindObjectsOfType<Assessor>()[0];
            _assessor.AddEffector(this);
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
        }

    }
}
