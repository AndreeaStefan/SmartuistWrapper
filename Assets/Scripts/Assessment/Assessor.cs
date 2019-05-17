using System.Collections.Generic;
using Effectors;
using UnityEngine;

namespace Assessment
{
    public class Assessor
    {
        private Dictionary<string, EndEffector> _effectors;
        
        
        public void RegisterEffector(EndEffector endEffector)
        {
            _effectors[endEffector.name] = endEffector;
        }

    }
    
}