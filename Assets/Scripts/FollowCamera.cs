using UnityEngine;
using InputTracking = UnityEngine.XR.InputTracking;

public class FollowCamera : MonoBehaviour
{
    public  GameObject target;
    void Start() 
    {
        InputTracking.disablePositionalTracking = false;
        var rotation = Helper.getYAxisRotation(transform.rotation);
//        target.transform.localRotation = Quaternion.Inverse(rotation);
        var transform1 = transform;
        var newPosition = transform1.position + transform1.forward * 2;
        newPosition.y = 0;
        target.transform.position = newPosition;
        target.transform.rotation = rotation;
    }

}
