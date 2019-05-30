using Effectors;

namespace Mapping.Types.ChangeDetectors
{
    public class SteadyPointing : ChangeDetector
    {

        private EndEffector _endEffector;
        

        public SteadyPointing(EndEffector effector)
        {
            _endEffector = effector;
        }
        
        public float IsChanging()
        {
            return 1.3f;
// _endEffector.transform.up
        }

        public void SetEndEffector(EndEffector endEffector)
        {
            _endEffector = endEffector;
        }
    }
}