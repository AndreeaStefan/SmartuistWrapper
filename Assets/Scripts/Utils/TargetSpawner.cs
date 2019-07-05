using System;
using UnityEngine;
using UnityEngine.Serialization;
using Assets.Scripts;
using Assessment;
using Assets.Scripts.Assessment;
using System.Collections.Generic;
using System.Linq;

public class TargetSpawner : MonoBehaviour
{
    public GameObject Floor;
    public GameObject Player;
    public Transform TargetContainer;
    public GameObject TargetPrefab;
   
    public bool Spiral;

    [Range(1f, 2f)] public float Height = 1.7f;

    private float _radius = 0.8f;
    private float[] _depths =  { 0.6f, 1.6f, 2.6f };
    private float[] _scales =  { 0.1f, 0.2f, 0.3f };
    [FormerlySerializedAs("_target")] public GameObject Target;
    [FormerlySerializedAs("CurrentTarget")] [HideInInspector] public int CurrentTargetID;

    public Target[] Targets;
    public Target CurrentTarget;

    private System.Random _randomGenerator;
    private int _batchSize;
    private List<int> _currentLesson;
    private int _currentRep;
    public int targetsPerCircle;
    public readonly int targersPerSpiralCircle = 3;
    public readonly int spiralDepths = 6;
    private int targetsPerCirclePerLesson;
    private int _currenDepths;


    void Start()
    {
        _randomGenerator = new System.Random(5);
        _batchSize = FindObjectOfType<Assessor>().BatchSize;

        if (Spiral)
            GenerateTargetsSpiral();
        else
            GenerateTargetsDifferentDepths();

        CurrentTarget = new Target();
        Target = Instantiate(TargetPrefab);
        Target.transform.parent = TargetContainer;
        Target.GetComponent<MeshRenderer>().enabled = false;
        _currenDepths = Spiral ? spiralDepths : _depths.Length;
        targetsPerCircle = Spiral ? 3 : 6;
        targetsPerCirclePerLesson = (int)Math.Ceiling((double)_batchSize / _currenDepths);// how many targets we take from each circle
        _currentLesson = Enumerable.Range(0, (int)targetsPerCirclePerLesson * _currenDepths).Select(i => -1).ToList();

    }

    public void GetNewTarget()
    {
        if (_batchSize == _currentRep)
        {
            _currentLesson = Enumerable.Range(0, targetsPerCirclePerLesson * _currenDepths).Select(i => -1).ToList();
            _currentRep = 0;
        }
        while (true)
        {
            var tmpIndex = _randomGenerator.Next(0, Targets.Length);
            var circle = (int)Math.Floor(tmpIndex / (decimal)targetsPerCircle);
            var position = tmpIndex % targetsPerCirclePerLesson;
            var index = targetsPerCirclePerLesson * circle + position;
            if (_currentLesson[index] == -1)
            {
                var scaleIndex = GetNewScale(index); 
                _currentLesson[index] = scaleIndex;
                CurrentTarget = Targets[tmpIndex];
                CurrentTarget.Scale = new Vector3(_scales[scaleIndex], _scales[scaleIndex], _scales[scaleIndex]);
                _currentRep++;
                return;
            }
        }
    }

    private int GetNewScale(int index)
    {
        var otherOne = index % 2 == 0 ? 1 : -1;
        while (true)
        {
            var scaleIndex = _randomGenerator.Next(0, _scales.Length);
            if (scaleIndex != _currentLesson[index + otherOne]) return scaleIndex;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void GenerateTargetsDifferentDepths()
    {
        var angle = 360 / targetsPerCircle;
        Targets = new Target[_depths.Length * targetsPerCircle];
      
        var count = 0;
        foreach (var d in _depths)
        {
            var center = Player.transform.position + Player.transform.forward * d;
            center.y = Height;
            for (var i = 0; i < targetsPerCircle; i++)
            {
                var a = i * angle;
                var pos = RandomCircle(center, _radius, a);
                Targets[count] = new Target
                {
                    Angle = a ,
                    Depth = d, 
                    Position = pos,
                    ID = count,
                };

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
        Targets = new Target[18];
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
            Targets[count] = new Target
            {
                Angle = a,
                Depth = d,
                Position = pos,
                ID = count,
            };
            count++;
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
