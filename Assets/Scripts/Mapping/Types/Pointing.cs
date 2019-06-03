using System.Collections.Generic;
using Effectors;
using Mapping.Types.ChangeDetectors;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Mapping.Types
{
    public class Pointing : MappingType
    {
        private ChangeDetector _changeDetector;
        private GameObject _bone;
        private float _initialYScale;

        public void Change(float how)
        {
            if (how > 0) Elongate(how);
            else Shorten(-how);
        }

        private void Elongate(float speed)
        {
            var newScale = _bone.transform.localScale;
            newScale.y = newScale.y + speed * Time.deltaTime;
            
            // todo: add counter scale for the children bones
            _bone.transform.localScale = newScale;
        }

        private void Shorten(float speed)
        {
            if (_initialYScale >= _bone.transform.localScale.y) return;
            
            var newScale = _bone.transform.localScale;
            newScale.y = newScale.y - speed * Time.deltaTime;
            
            // todo: add counter scale for the children bones
            _bone.transform.localScale = newScale;
        }
        

        public float IsChanging()
        {
            return _changeDetector.IsChanging();
        }

        public void SetBone(GameObject newBone)
        {
            _bone = newBone;
            _initialYScale = _bone.transform.localScale.y;
        }

        public void SetChangeDetector(ChangeDetector changeDetector)
        {
            _changeDetector = changeDetector;
        }
    }
}