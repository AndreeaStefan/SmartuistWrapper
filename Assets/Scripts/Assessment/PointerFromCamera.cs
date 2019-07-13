using Assessment;
using System.Diagnostics;
using UnityEngine;
namespace Assets.Scripts.Assessment
{
    public class PointerFromCamera : MonoBehaviour
    {

        public GameObject cursorPrefab;

        [HideInInspector]  public bool enabled;
       

        private float maxCursorDistance = 10000;
        private Stopwatch _stopwatch;
        private readonly int focusTime = 1000;
        public Camera camera;

        private GameObject cursorInstance;
        private Questionnaire _questionnaire;

        public PointerFromCamera()
        {
            _stopwatch = new Stopwatch();

        }

        void Update()
        {
            if (enabled)
            {
                if (cursorInstance == null)
                    cursorInstance = Instantiate(cursorPrefab);

                if (_questionnaire == null)
                    _questionnaire = FindObjectOfType<Questionnaire>();

                RaycastHit hit;
                UnityEngine.Debug.DrawRay(camera.transform.position, camera.transform.forward * 2, Color.blue);
                Ray ray = new Ray(camera.transform.position, camera.transform.forward * 2);

                 if (Physics.Raycast(ray, out hit, 20) && !hit.transform.tag.Equals("Start"))
                 {
                     Transform objectHit = hit.transform;

                     cursorInstance.transform.position = hit.point;
                     cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                     if (objectHit.tag.Equals("Evaluation"))
                     {

                         if (!_stopwatch.IsRunning)
                         {
                            _stopwatch.Start();
                             return;
                         }

                         if (_stopwatch.Elapsed.TotalMilliseconds >= focusTime)
                         {
                             _questionnaire.HitTarget(hit.transform.gameObject);
                             _stopwatch.Restart();

                        }

                     }
                     else
                     {
                         _stopwatch.Restart();
                     }
                 }
                 else
                 {
                     cursorInstance.transform.position = ray.origin + ray.direction.normalized * maxCursorDistance;
                     cursorInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, -ray.direction);
                 }
            }
            else
            {
                if (cursorInstance != null)
                    Destroy(cursorInstance);
            }
        }
    }
}