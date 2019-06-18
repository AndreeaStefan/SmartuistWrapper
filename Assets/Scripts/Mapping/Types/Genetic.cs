using System;
using System.Collections.Generic;
using System.IO;
using Mapping.Types.ChangeDetectors;
using UnityEngine;

namespace Mapping.Types
{
    public class Genetic : MappingType
    {
        
        private readonly string fileName = "lengths.txt";
        private StreamReader sReader;
        private List<string> lengths;
        private Academy _academy;
        private GameObject _bone;
        private GameObject _secondBone;
        private float _initialYScale;

        public Genetic(Academy a)
        {
            _academy = a;
            lengths = new List<string>();
            sReader = new StreamReader(new FileStream(fileName, FileMode.Open, 
                FileAccess.Read, FileShare.ReadWrite));
            
        }
        
        
        public void Change(float how)
        {
            _academy.newTarget = false;
            var line = sReader.ReadLine();
            if (line == null) return;
            var scale = int.Parse(line)/100f;
            var newScale = _bone.transform.localScale;
            newScale.y = scale;
            _bone.transform.localScale = newScale;
            _secondBone.transform.localScale = newScale;
        }

        public float IsChanging()
        {
            return _academy.newTarget ? 1 : 0;
        }

        public void SetChangeDetector(ChangeDetector changeDetector)
        {
        }

        public void SetBone(GameObject newBone)
        {
            _bone = newBone;
            _initialYScale = _bone.transform.localScale.y;
        }

        public void SetSecondBone(GameObject newBone)
        {
            _secondBone = newBone;
        }
    }
}