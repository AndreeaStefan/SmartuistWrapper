using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TrackerPositionHandler : MonoBehaviour
{
    [FormerlySerializedAs("target")] public GameObject PlayerContainer;


    // Update is called once per frame
    void Update()
    {
        var newPosition = transform.position;
        newPosition.y = PlayerContainer.transform.position.y;
        PlayerContainer.transform.position = newPosition;

    }
}
