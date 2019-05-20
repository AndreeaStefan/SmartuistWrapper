using UnityEngine;
using UnityEngine.UIElements;

public class FollowCamera : MonoBehaviour
{
    public  GameObject target;
    public float distanceFromCamera = -2;
    public float deltaTime = 0.05f;

    private void Update()
    {
        var currentTransform = transform;
        var point = target.transform.position;
        point.y = currentTransform.position.y;
        var angle = Vector3.SignedAngle(currentTransform.forward, target.transform.forward, Vector3.up);
        //  Debug.Log("Rotating to align with camera, angle: " + angle);
        //transform.RotateAround(point, Vector3.up, angle * deltaTime);

        Vector3 resultingPosition = target.transform.position - target.transform.forward * distanceFromCamera;
        transform.position = new Vector3(resultingPosition.x, transform.position.y, resultingPosition.z);
        Vector3 relativePos = target.transform.position - transform.position;
        relativePos.y = 0;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;
      


      

    }
}
