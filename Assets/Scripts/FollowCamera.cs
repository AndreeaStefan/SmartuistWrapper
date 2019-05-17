using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public  GameObject target;

    private void Update()
    {
        var currentTransform = transform;
        var point = target.transform.position;
        point.y = currentTransform.position.y;
      //  transform.RotateAround(point, Vector3.up, Vector3.Angle(currentTransform.forward, target.transform.forward) * Time.deltaTime);
    }
}
