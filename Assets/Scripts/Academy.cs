using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Academy : MonoBehaviour
{

    public GameObject Tracker;
    public GameObject Room;
    public GameObject Camera;

    void Awake()
    {
        // aligning the room with the headset        
        var position = Tracker.transform.position;
        position.y = 0f;
        Room.transform.position = position;
        var rotation = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Room.transform.rotation = rotation;
        
        // 
    }
}
