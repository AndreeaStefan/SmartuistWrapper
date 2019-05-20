using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Academy : MonoBehaviour
{

    public GameObject Tracker;
    public GameObject Room;

    void Awake()
    {
        Room.transform.position = Tracker.transform.position;
    }
}
