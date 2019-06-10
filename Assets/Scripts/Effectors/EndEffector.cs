using System;
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
        private Stopwatch stopwatch;

        // Start is called before the first frame update
        void Start()
        {
            name = $"EndEffector_{transform.parent.name}";
            actor.AddEffector(this);

            _assessor = FindObjectsOfType<Assessor>()[0];
            _assessor.AddEffector(this);

            stopwatch = Stopwatch.StartNew();
        }

        public void Initialise(Assessor assessor)
        {
            _assessor = assessor;
        }
        

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log( $"{name} triggered on Enter {other.name} at: {DateTime.Now}");
            if (other.transform.gameObject.CompareTag("Target"))
            {
                Debug.Log($"{name} hit target");
                if (actor.SpawnTargets != null)
                {

                    stopwatch.Stop();
                    var elapsedMs = stopwatch.ElapsedMilliseconds;
                    var result = new Result(other.transform.localScale.x, 
                                            actor.SpawnTargets.CurrentTargetDepth, 
                                            elapsedMs,
                                            actor.academy.Mapping.ToString());

                    _assessor.AddResult(result, this);

                    Debug.Log($"{name} hit target,  size {result.targetSize}, depth {result.targetDepth}, time {result.movementTime}");

                    var newPos = actor.SpawnTargets.GetNewPosition();
                    var newScale = actor.SpawnTargets.GetNewScale();
                    other.transform.position = newPos;
                    other.transform.localScale = newScale;

                    stopwatch.Restart();
                   

                }
                    
            }
        }
        
        
    }
}
