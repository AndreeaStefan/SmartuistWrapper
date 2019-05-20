using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetSpawner : MonoBehaviour
{
    public GameObject Floor;
    public GameObject Player;
    public Transform TargetContainer;
    public GameObject TargetPrefab;

    [Range(1, 15)] public int NumberOfTargets;
    [Range(0.1f, 1)] public float ScaleOfTargets;
    [Range(0.2f, 5f)] public float MinDistanceFromPlayer = 1;
    [Range(0.2f, 5f)] public float MaxDistanceFromPlayer = 5;
    [Range(1f, 2f)] public float Height = 1.7f;

    private Bounds _areaBounds;

    void Awake()
    {
        _areaBounds = Floor.GetComponent<Collider>().bounds;

        // spawn the targets
        for (int i = 0; i < NumberOfTargets; i++)
        {
            var target = Instantiate(TargetPrefab) as GameObject;
            target.transform.parent = TargetContainer;
            target.transform.position = GetNewPosition();
            target.transform.localScale = new Vector3(ScaleOfTargets, ScaleOfTargets, ScaleOfTargets);
        }
    }

    /// <summary>
    /// Get a new position in the area for the target
    /// Make sure that is in a "safe" distance from the player
    ///  </summary>
    public Vector3 GetNewPosition()
    {
        var position = GetRandomSpawnPosition();
        return position;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            var randomPosX = Random.Range(-MaxDistanceFromPlayer, MaxDistanceFromPlayer);
            var randomPosZ = Random.Range(-MaxDistanceFromPlayer, MaxDistanceFromPlayer);
            var randomPosY = Random.Range(0.5f, Height + Height / 2);

            randomSpawnPos = Player.transform.position + new Vector3(randomPosX, 0, randomPosZ);
            randomSpawnPos.y = randomPosY;

            // Checks if not colliding with anything
            if (Physics.CheckBox(randomSpawnPos, new Vector3(0.5f, 0.5f, 0.5f)) == false)
            {
                //check if it is in bounds and not too close to the player
                if (_areaBounds.min.x < randomPosX && _areaBounds.max.x > randomPosX &&
                    _areaBounds.min.z < randomPosZ && _areaBounds.max.z > randomPosZ &&
                    Math.Abs(randomPosX) > MinDistanceFromPlayer && Math.Abs(randomPosZ) > MinDistanceFromPlayer)
                {
                    foundNewSpawnLocation = true;
                }
            }

        }

        return randomSpawnPos;

    }
}
