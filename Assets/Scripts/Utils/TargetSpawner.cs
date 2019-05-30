using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class TargetSpawner : MonoBehaviour
{
    public GameObject Floor;
    public GameObject Player;
    public Transform TargetContainer;
    public GameObject TargetPrefab;
    public bool Spiral;

    [Range(1, 15)] public int NumberOfTargets;
    [Range(1f, 2f)] public float Height = 1.7f;

    private Bounds _areaBounds;
    private float _radius = 1.5f;
    private int[] _depths = new int[3] { 3, 5, 7 };
    private float[] _scales = new float[3] { 0.3f, 0.5f, 0.7f };
    private GameObject _target;
    

    private Vector3[] _targetPositions;
    private System.Random _randomGenerator;

    void Awake()
    {
        _randomGenerator = new System.Random(5);
        _areaBounds = Floor.GetComponent<Collider>().bounds;

        if (Spiral)
            GenerateTargetsSpiral();
        else 
            GenerateTargetsDifferentDepths();
      
        _target = Instantiate(TargetPrefab) as GameObject;
        _target.transform.parent = TargetContainer;
        _target.transform.position = _targetPositions[0];
        _target.transform.localScale = new Vector3(_scales[0], _scales[0], _scales[0]);
      
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            var index = _randomGenerator.Next(0, _targetPositions.Length);
            var scale = _randomGenerator.Next(0, _scales.Length);
            Debug.Log(index);
            var pos = _targetPositions[index];
            _target.transform.position = pos;
            _target.transform.localScale = new Vector3(_scales[scale], _scales[scale], _scales[scale]);
        }
    }

    public Vector3 GetNewPosition()
    {
        var index = _randomGenerator.Next(0, _targetPositions.Length);
        return _targetPositions[index];
    }

    public Vector3 GetNewScale()
    {
        var index = _randomGenerator.Next(0, _scales.Length);
        return new Vector3(_scales[index], _scales[index], _scales[index]);
    }

    /// <summary>
    /// 
    /// </summary>
    private void GenerateTargetsDifferentDepths()
    {
        var targetsPerCircle = 6;
        var angle = 360 / targetsPerCircle;
        _targetPositions = new Vector3[_depths.Length * targetsPerCircle];
        var count = 0;
        foreach (var d in _depths)
        {
            var center = Player.transform.position + Player.transform.forward * d;
            for (var i = 0; i < targetsPerCircle; i++)
            {
                var a = i * angle;
                var pos = RandomCircle(center, _radius, a);
                _targetPositions[count] = pos;
                count++;
            }
        }
    }

    /// <summary>
    /// Generates position for targets such that they form a spiral
    /// </summary>
    private void GenerateTargetsSpiral()
    {
        var nrTargets = 18;
        _targetPositions = new Vector3[_depths.Length * nrTargets];
        _radius = 0.8f; // radius of the initial circle - increases after a full circle is done 
        var angle = 360 / 6; //targets are placed at angles: 0, 60, 120....
        var count = 0;
        var d = 3f; // depth - increases with every new target
        var cnt = 0; 
        for (var i = 0; i < nrTargets; i++)
        {
            var center = Player.transform.position + Player.transform.forward * d;
            
            var a = cnt * angle;
            cnt += 1;
            d += 0.3f;
            
            if (cnt == 6)
            {
                cnt = 0;
                _radius += 0.4f;
            }
            var pos = RandomCircle(center, _radius, a);
            _targetPositions[count] = pos;
            count++;
        }

    }

    Vector3 RandomCircle(Vector3 center, float radius, int a)
    {
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad) + 1;
        pos.z = center.z;
        return pos;
    }
}
