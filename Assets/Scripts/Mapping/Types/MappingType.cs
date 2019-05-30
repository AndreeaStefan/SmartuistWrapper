using Effectors;
using Mapping.Types.ChangeDetectors;
using UnityEngine;

namespace Mapping.Types
{
    public interface MappingType
    {
        void Change(float how);
        /// <summary>
        /// Assessing whether the bone should change its size
        /// </summary>
        /// <returns>
        /// Returns int in range [-10;10] where negative value means shortening. The speed of change is indicated by the number; 0 means no change
        /// </returns>
        float IsChanging();
        void SetChangeDetector(ChangeDetector changeDetector);
        void SetBone(GameObject bone);
    }
}