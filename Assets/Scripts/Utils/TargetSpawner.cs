using Assets.Scripts.Utils;
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
    private float _radius = 0.8f;
    private float[] _depths = new float[3] { 1, 1.8f, 2.6f };
    private float[] _scales = new float[3] { 0.2f, 0.3f, 0.4f };
    private GameObject _target;

    public float CurrentTargetDepth;
    private float[] _targetDepths;
    
    private Vector3[] _targetPositions;
    private System.Random _randomGenerator;

    [HideInInspector] public int CurrentTarget;

    void Start()
    {
        _randomGenerator = new System.Random(5);
        _areaBounds = Floor.GetComponent<Collider>().bounds;

        if (Spiral)
            GenerateTargetsSpiral();
        else
            GenerateTargetsDifferentDepths();

          _target = Instantiate(TargetPrefab);
           _target.transform.parent = TargetContainer;
           _target.transform.position = GetNewPosition();
          _target.transform.localScale = GetNewScale();

          CurrentTargetDepth = _targetDepths[1];

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
        CurrentTarget = index;
        CurrentTargetDepth = _targetDepths[index];
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
        _targetDepths = new float[_depths.Length * targetsPerCircle];
        var count = 0;
        foreach (var d in _depths)
        {
            var center = Player.transform.position + Player.transform.forward * d;
            center.y = Height;
            for (var i = 0; i < targetsPerCircle; i++)
            {
                var a = i * angle;
                var pos = RandomCircle(center, _radius, a);
                _targetPositions[count] = pos;
                _targetDepths[count] = d;
                count++;

                /*
                var target = Instantiate(TargetPrefab) as GameObject;
                target.transform.parent = TargetContainer;
                target.transform.position = pos;
                target.transform.localScale = new Vector3(_scales[0], _scales[0], _scales[0]);
                */

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
        _targetDepths = new float[_depths.Length * nrTargets];
        _radius = 0.3f; // radius of the initial circle - increases after a full circle is done 
        var angle = 360 / 6; //targets are placed at angles: 0, 60, 120....
        var count = 0;
        var d = 0.5f; // depth - increases with every new target
        var cnt = 0;
        for (var i = 0; i < nrTargets; i++)
        {
            var center = Player.transform.position + Player.transform.forward * d;
            center.y = Height ;

            var a = cnt * angle;
            cnt += 1;
            d += 0.12f;

            if (cnt == 6)
            {
                cnt = 0;
                _radius += 0.2f;
            }
            var pos = RandomCircle(center, _radius, a);
            _targetPositions[count] = pos;
            _targetDepths[count] = d;
            count++;

            /*
               var target = Instantiate(TargetPrefab) as GameObject;
               target.transform.parent = TargetContainer;
               target.transform.position = pos;
               target.transform.localScale = new Vector3(_scales[0], _scales[0], _scales[0]);
               */
        }

    }

    Vector3 RandomCircle(Vector3 center, float radius, int a)
    {
        float ang = a;
        Vector3 pos;
        pos.x = center.x;
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        return pos;
    }
}
