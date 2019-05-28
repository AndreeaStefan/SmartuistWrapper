using UnityEngine;

public class Academy : MonoBehaviour
{

    public GameObject Tracker;
    public GameObject Room;
    public GameObject Camera;
    public GameObject PlayerContainer;
    public GameObject Source;
    [Range(-1,5)] public float DistanceFromCamera = 2;

    [Range(0f, 50f)]public float CameraRotationSpeed;
    void Start()
    {
        // aligning the room with the headset        
//        var position = Tracker.transform.position;
//        position.y = 0f;
//        Room.transform.position = position;
//        var rotation = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
//        Room.transform.rotation = rotation;
        
        // 
    }
    
    public void RelocateTheArena()
    {
        var position = Tracker.transform.position;
        position.y = 0f;
        Room.transform.position = position;
    }
}
