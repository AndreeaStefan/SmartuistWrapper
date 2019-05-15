using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerPositionHandler : MonoBehaviour
{
    public GameObject target;


    // Update is called once per frame
    void Update()
    {
        var newPosition = transform.position;
        newPosition.y = target.transform.position.y;
        target.transform.position = newPosition;

    }
}
