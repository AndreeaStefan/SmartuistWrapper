using System.Collections.Generic;
using UnityEngine;

namespace Mapping
{
    public interface IBonesMapping 
    {
        List<GameObject> Bones();
        int RootBone();
    }
}