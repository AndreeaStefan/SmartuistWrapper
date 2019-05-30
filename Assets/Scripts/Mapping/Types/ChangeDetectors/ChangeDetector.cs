using Effectors;

namespace Mapping.Types.ChangeDetectors
{
    public interface ChangeDetector
    {
        float IsChanging();
        void SetEndEffector(EndEffector endEffector);
    }
}