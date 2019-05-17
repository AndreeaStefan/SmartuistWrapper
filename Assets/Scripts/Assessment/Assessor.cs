using System.Collections.Generic;
using Effectors;
using UnityEngine;

namespace Assessment
{
    public class Assessor
    {
        private readonly Dictionary<EndEffector, Result> _effectors = new Dictionary<EndEffector, Result>();
        
        public void AddEffector(EndEffector effector)
        {
            _effectors[effector] = new Result();
            effector.Initialise(this);
        }
    }
    
}