using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Academy : MonoBehaviour
{

    public GameObject Room;
    public GameObject Tracker;
    [FormerlySerializedAs("Room")] public GameObject PlayerContainer;
    public GameObject Camera;

    [Range(0f, 50f)]public float CameraRotationSpeed;
    void Start()
    {
        // aligning the room with the headset        

//        var rotation = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
//        PlayerContainer.transform.rotation = rotation;
    }
    
    public void RelocateTheArena()
    {
        var position = Tracker.transform.position;
        position.y = 0f;
        Room.transform.position = position;
    }
}
