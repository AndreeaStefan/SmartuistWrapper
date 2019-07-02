using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assessment
{
    public class Questionnaire : MonoBehaviour
    {
        public bool Enabled;
        
        private static GameObject _container;
        private static GameObject _points;
        private static Assessor _assessor;
        private static readonly List<string> _questions = new List<string>
        {
            "How physically demanding was the task?",
            "How did you feel with this arm's length (comfortable, annoyed)?",
            "How successful do you think you were?"
        };
        private static List<string> _results = new List<string>();

        private static int _currentQuestion;
        private static TextMesh _textMesh;
        
        private Renderer _renderer;
        private Color _initialColour;

        private bool _hitsCount = true;
        
        private void Start()
        {
            _assessor = FindObjectOfType<Assessor>();
            _container = gameObject.transform.GetChild(0).gameObject;
            _points = _container.transform.GetChild(1).gameObject;
            _container.SetActive(false);
            _textMesh = transform.GetChild(0).Find("Question").GetComponent<TextMesh>();
        }

        public void StartQuestionnaire()
        {
            _results = new List<string>(_questions.Count);
            if (!Enabled)
            {
                _assessor.DoneQuestionnaire(string.Join(",", _results));
                return;
            }
            
            _currentQuestion = 0;
            _textMesh.text = _questions[_currentQuestion];
            _container.SetActive(true);
            
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
        }
        
    }
}