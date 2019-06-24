using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Assessment;
using Mapping.Types.ChangeDetectors;
using UnityEditor.UIElements;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Assets.Scripts.Mapping.Types.ChangeDetectors
{
    public class LearnStaticChange : ChangeDetector
    {
        private Assessor _assessor;
        private float _currentScale;
        private int _lesson;

        public LearnStaticChange(Assessor assessor, float scale)
        {
            _assessor = assessor;
            _lesson = 0;
            _currentScale = scale;
        }

        public float IsChanging()
        {
            if (_assessor.CurrentLessonNr != _lesson)
            {
                _lesson = _assessor.CurrentLessonNr;
                var nextScale = _assessor.Scale;
                _currentScale = nextScale;
                UnityEngine.Debug.Log("Next Scale" + nextScale);
                return nextScale;
            }
            return _currentScale;         
        }
    }
}

