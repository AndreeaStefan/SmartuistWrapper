using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assessment;
using UnityEngine;

namespace Assessment
{
    public class Questionnaire : MonoBehaviour
    {
        public bool Enabled;
        
        private static GameObject _container;
        private static GameObject _points;
        private static Assessor _assessor;
        private FacingChecker _facignChecker;
        private static readonly List<string> _questions = new List<string>
        {
            "How physically demanding was the task? \n1(very low) ... 7(very high)",
            "How successful were you in tapping targets \n1(perfect) .. 7(failure)?",
            "How discuraged, stressed, annoyed were you with this arm's length? \n1(very low) ... 7(very high)" // the scale should increase the same way
        };
        private static List<string> _results = new List<string>();

        private static int _currentQuestion;
        private static TextMesh _textMesh;
        
        private Renderer _renderer;
        private Color _initialColour;

        private bool _hitsCount = true;
        private PointerFromCamera _pointer;

        private void Start()
        {
            _assessor = FindObjectOfType<Assessor>();
            _facignChecker = FindObjectOfType<FacingChecker>();
            _container = gameObject.transform.GetChild(0).gameObject;
            _points = _container.transform.GetChild(1).gameObject;
            _container.SetActive(false);
            _textMesh = transform.GetChild(0).Find("Question").GetComponent<TextMesh>();
            if ((_pointer = FindObjectOfType<PointerFromCamera>()) == null)
                 _pointer = gameObject.AddComponent(typeof(PointerFromCamera)) as PointerFromCamera;
            _pointer.camera = _assessor.camera;

            var pos = transform.position;
            pos.y = _assessor.ParticipantHeight;
            transform.position = pos;
        }

        public void StartQuestionnaire()
        {
            _facignChecker.ActivateCountdown(ReadyForQuestions,0);
        }

        public void ReadyForQuestions()
        {
            _results = new List<string>(_questions.Count);
            if (!Enabled)
            {
                StartCoroutine(nameof(FinishQuestionnaire));
                return;
            }

            _currentQuestion = 0;
            _textMesh.text = _questions[_currentQuestion];
            _container.SetActive(true);


            _pointer.enabled = true;
        }

        public void HitTarget(GameObject result)
        {
            if (!_hitsCount) return;
            
            _results.Add(result.name);
            _currentQuestion++;
            StartCoroutine(nameof(ContinueQuestions), result.gameObject);
            if (_currentQuestion == _questions.Count)
            {
                StartCoroutine(nameof(FinishQuestionnaire));
                return;
            }
            _textMesh.text = _questions[_currentQuestion];
        }
        
        private IEnumerator ContinueQuestions(GameObject obj)
        {
            if (!_hitsCount) yield break;
            
            _hitsCount = false;
            _renderer = obj.GetComponent<Renderer>();
            _initialColour = _renderer.material.color;
            obj.GetComponent<Renderer>().material.color = Color.green;
            yield return new WaitForSeconds(0.2f);
            _renderer.material.color = _initialColour;
            _points.SetActive(false);
            StartCoroutine(nameof(EnablePoints));
        }

        private IEnumerator EnablePoints()
        {
            yield return new WaitForSeconds(1f);
            _points.SetActive(true);
            _hitsCount = true;
        }

        private IEnumerator FinishQuestionnaire()
        {
            yield return new WaitForSeconds(1f);
            _container.SetActive(false);
            _assessor.DoneQuestionnaire(string.Join(",", _results));
            _pointer.enabled = false;
        }
        
    }
}