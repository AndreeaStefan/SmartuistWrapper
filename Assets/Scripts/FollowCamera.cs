using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public  GameObject target;

    private void Update()
    {
        var currentTransform = transform;
        var point = target.transform.position;
        point.y = currentTransform.position.y;
        var angle = Vector3.SignedAngle(currentTransform.forward, target.transform.forward, Vector3.up);
      //  Debug.Log("Rotating to align with camera, angle: " + angle);
      transform.RotateAround(point, Vector3.up, angle * Time.deltaTime);
    }
}
