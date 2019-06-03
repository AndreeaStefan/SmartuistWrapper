using UnityEngine;
using Valve.VR;

public class Academy : MonoBehaviour
{
    public GameObject Room;
    public GameObject Root;

    void Awake()
    {
        ResizeFloor();
    }

    private void ResizeFloor()
    {
        float x = 0, z = 0;
        OpenVR.Chaperone.GetPlayAreaSize(ref x, ref z);

        var floor = GameObject.Find("Floor");
        var currentScale = floor.transform.localScale;
        currentScale.x = x;
        currentScale.z = z;

        floor.transform.localScale = currentScale;

        var targetSpawner = FindObjectsOfType<TargetSpawner>()[0];
        targetSpawner.Floor = floor;
    }
}