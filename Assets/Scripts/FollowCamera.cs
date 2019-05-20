using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public  GameObject target;
    private Academy _academy;
    
    
    private void Start()
    {
        _academy = FindObjectOfType<Academy>();
    }
    
    
    private void Update()
    {
        var currentTransform = transform;
        var point = target.transform.position;
        point.y = currentTransform.position.y;
        var angle = Vector3.SignedAngle(currentTransform.forward, target.transform.forward, Vector3.up);
        Debug.DrawRay(target.transform.position, target.transform.forward, Color.green);
        Debug.DrawRay(target.transform.position, Vector3.up, Color.red);
        Debug.DrawRay(currentTransform.position, currentTransform.forward, Color.green);
      //  Debug.Log("Rotating to align with camera, angle: " + angle);
        transform.RotateAround(point, Vector3.up, angle * Time.fixedDeltaTime * _academy.CameraRotationSpeed);
    }
}
