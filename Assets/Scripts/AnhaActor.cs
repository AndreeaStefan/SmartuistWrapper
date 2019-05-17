
using System;
using System.Collections.Generic;
using System.Linq;
using Assessment;
using Assets.Scripts.Mapping;
using Assets.Scripts.Utils;
using Effectors;
using Mapping;
using Rokoko.Smartsuit;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class AnhaActor : MonoBehaviour
{
    public SmartsuitActor actor;
    public BasicBoneMapping bonesType ;
    public Transform floor;

    [Range(0, 180)]
    public float armsExtensionX = 0;

    [Range(0, 90)]
    public float legsExtensionZ = 0;

    [Range(0, 2)]
    public float scaleArms = 1;

    [Range(0, 2)]
    public float scaleLegs = 1;

    [Range(0, 2)]
    public float scaleBody = 1;


    public TargetSpawner SpawnTargets;

    private List<GameObject> _bones;
    private Quaternion[] _poseOffsets;
    private Pose _pose;
    private Quaternion[] _quaternionArray;
    private Vector3[] _scaleArray;
    private float[] _limits;

    private Dictionary<GameObject, Quaternion> _initialRot;
    private int count;
    [FormerlySerializedAs("_assessor")] public Assessor assessor;
    
    private void Start()
    {
        if(assessor == null) assessor = new Assessor();
        _bones = bonesType.Bones();
        _initialRot = new Dictionary<GameObject, Quaternion>();
        InitialisePose();
    }

        private void InitialisePose()
    {
        _pose = new Pose();
        _pose.Store(bonesType);
        _pose.forward = transform.forward;
        _poseOffsets = _pose.ExtractRotationOffsets();
    }

    private void GetAngle()
        { }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    private void Move()
    {
        var rotation = transform.rotation;

        // init 
        if (count == 0)
        {
           
            for (var index = 0; index < _poseOffsets.Length; ++index)
            {
                var rr = rotation * actor.CurrentState.sensors[index].UnityQuaternion;
                _initialRot.Add(_bones[index], rr * _poseOffsets[index]);
            }

            count++;
        }

        
        _quaternionArray = new Quaternion[_poseOffsets.Length];
        _scaleArray = new Vector3[_poseOffsets.Length];
        _limits = new float[_poseOffsets.Length];

        var armIndices = Helper.GetArmIndices();
        var legIndices = Helper.GetLegIndices();
        var mainIndices = Helper.GetMainBodyIndices();
        var upperLegsIndices = Helper.GetUpperLegIndices();

        MoveSection(armIndices, armsExtensionX, 0, 0);
        MoveSection(legIndices, 0, 0, legsExtensionZ);
        MoveSection(upperLegsIndices, 0, 0, 0);
        MoveSection(mainIndices, 0, 0, 0);

        ScaleSection(armIndices, 1, scaleArms, 1);
        ScaleSection(legIndices, 1, scaleLegs, 1);
        ScaleSection(mainIndices, 1, scaleBody, 1);

        _quaternionArray[7] = Quaternion.Lerp(_quaternionArray[0], _quaternionArray[8], 0.5f);

        for (var index = 0; index < _poseOffsets.Length; ++index)
        {
    
            if (_bones[index] != null && index < _quaternionArray.Length)
            {
                _bones[index].transform.rotation = _quaternionArray[index] * _poseOffsets[index];
                _bones[index].transform.localScale = _scaleArray[index];
            } 
        }
               
        if ((uint) actor.CurrentState.sensors.Length <= 0U)
            return;
            
        var vector3 = transform.TransformPoint(actor.CurrentState.sensors[0].UnityPosition);
        if (!float.IsNaN(vector3.x) && !float.IsNaN(vector3.y) && !float.IsNaN(vector3.z) && (bool) _bones[bonesType.RootBone()])
            _bones[bonesType.RootBone()].transform.position = vector3;
    }

    private void MoveSection(List<int> indices, float extX, float extY, float extZ)
    {
        var rotation = transform.rotation;
        for (var i = 0; i < indices.Count; i++)
        {
            var index = indices[i];
            // TODO: here introduce index change to have sensor mapped onto different body part
            if ( _bones[index] != null && index < _quaternionArray.Length)
            {
                var rotFromTheSuit = rotation * actor.CurrentState.sensors[index].UnityQuaternion ;

                var euler = rotFromTheSuit.eulerAngles;
                var rotX = euler.x + Mathf.Sign(euler.x) * extX;
                var rotY = euler.y + Mathf.Sign(euler.y) * extY; //Mathf.Lerp(euler.y, euler.y * extY, Time.time * 0.1f);
                var rotZ = euler.z + Mathf.Sign(euler.z) * extZ;
                var nextRot = Quaternion.Euler(rotX, rotY, rotZ);
                var angle = Math.Abs(Quaternion.Angle(_initialRot[_bones[index]], rotFromTheSuit));

                _quaternionArray[index] = rotFromTheSuit;
            }
        }
    }

    private void ScaleSection(List<int> indices, float extX, float extY, float extZ)
    {
        for (var i = 0; i < indices.Count; i++)
        {
            var index = indices[i];
            if (_bones[index] != null && index < _quaternionArray.Length)
            {
                _scaleArray[index] = new Vector3(extX, extY, extZ);
            }
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    public void AddEffector(EndEffector endEffector)
    {
        if(assessor == null) assessor = new Assessor();
        assessor.AddEffector(endEffector);
    }

}
