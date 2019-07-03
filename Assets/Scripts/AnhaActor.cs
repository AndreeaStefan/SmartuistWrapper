using System;
using System.Collections;
using System.Collections.Generic;
using Assessment;
using Assets.Scripts.Mapping;
using Assets.Scripts.Utils;
using Assets.Scripts.Mapping.Types;
using Effectors;
using Mapping;
using Mapping.Types;
using Rokoko.Smartsuit;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AnhaActor : MonoBehaviour
{
    public SmartsuitActor actor;
    public BasicBoneMapping bonesType;
    public Academy academy;
    public Camera camera;

    [HideInInspector] public List<GameObject> Bones;
    private Quaternion[] _poseOffsets;
    private Pose _pose;
    private Quaternion[] _quaternionArray;

    private Dictionary<GameObject, Quaternion> _initialRot;
    private int count;
    [FormerlySerializedAs("_assessor")][HideInInspector] public Assessor assessor;
    public List<Mapper> mappers;
    private Text _text;

    private bool savePosition;
    
    [HideInInspector]  public Vector3 PreviousPosition;

    private void Start()
    {
        if (assessor == null) assessor = FindObjectOfType<Assessor>();
        if (mappers == null) mappers = new List<Mapper>();
        _text = FindObjectOfType<Text>();
        Bones = bonesType.Bones();
        _initialRot = new Dictionary<GameObject, Quaternion>();
        InitialisePose();
        transform.position = actor.transform.position;

        var startPos = GameObject.FindWithTag("Start").transform.position;
        startPos.y = 1.5f;
        PreviousPosition = startPos; //Bones[bonesType.RootBone()].transform;  

    }

    private void InitialisePose()
    {
        _pose = new Pose();
        _pose.Store(bonesType);
        _pose.forward = transform.forward;
        _poseOffsets = _pose.ExtractRotationOffsets();
    }


    // Update is called once per frame
    void Update()
    {
        transform.position = actor.transform.position;
        transform.rotation = actor.transform.rotation;
        Move();
//        if (savePosition)
//            assessor.SaveBaselineRecord(Helper.GetPositionRecord(actor.CurrentState));
    }

    public Transform GetRoot()
    {
        return Bones[bonesType.RootBone()].transform;
    }

    private void Move()
    {
        if ((uint) actor.CurrentState.sensors.Length <= 0U)
            return;
        var rotation = transform.rotation;

        // init 
        if (count == 0)
        {
            for (var index = 0; index < _poseOffsets.Length; ++index)
            {
                var rr = rotation * actor.CurrentState.sensors[index].UnityQuaternion;
                _initialRot.Add(Bones[index], rr * _poseOffsets[index]);
            }

            count++;
        }

        _quaternionArray = new Quaternion[_poseOffsets.Length];

        var armIndices = Helper.GetArmIndices();
        var legIndices = Helper.GetLegIndices();
        var mainIndices = Helper.GetMainBodyIndices();
        var upperLegsIndices = Helper.GetUpperLegIndices();

        MoveSection(armIndices, 0, 0, 0);
        MoveSection(legIndices, 0, 0, 0);
        MoveSection(upperLegsIndices, 0, 0, 0);
        MoveSection(mainIndices, 0, 0, 0);

        _quaternionArray[7] = Quaternion.Lerp(_quaternionArray[0], _quaternionArray[8], 0.5f);

        for (var index = 0; index < _poseOffsets.Length; ++index)
        {
            if (Bones[index] != null && index < _quaternionArray.Length)
            {
                Bones[index].transform.rotation = _quaternionArray[index] * _poseOffsets[index];
            }
        }


        var vector3 = transform.TransformPoint(actor.CurrentState.sensors[0].UnityPosition);
        if (!float.IsNaN(vector3.x) && !float.IsNaN(vector3.y) && !float.IsNaN(vector3.z) && (bool) Bones[bonesType.RootBone()])
            Bones[bonesType.RootBone()].transform.position = vector3;
    }

    private void MoveSection(List<int> indices, float extX, float extY, float extZ)
    {
        var rotation = transform.rotation;
        for (var i = 0; i < indices.Count; i++)
        {
            var index = indices[i];
            // TODO: here introduce index change to have sensor mapped onto different body part
            if ( Bones[index] != null && index < _quaternionArray.Length)
            {
                var rotFromTheSuit = rotation * actor.CurrentState.sensors[index].UnityQuaternion ;

                var euler = rotFromTheSuit.eulerAngles;
                var rotX = euler.x + Mathf.Sign(euler.x) * extX;
                var rotY = euler.y + Mathf.Sign(euler.y) * extY; //Mathf.Lerp(euler.y, euler.y * extY, Time.time * 0.1f);
                var rotZ = euler.z + Mathf.Sign(euler.z) * extZ;
                var nextRot = Quaternion.Euler(rotX, rotY, rotZ);
                var angle = Math.Abs(Quaternion.Angle(_initialRot[Bones[index]], rotFromTheSuit));

                _quaternionArray[index] = rotFromTheSuit;
            }
        }
    }
    
    public void AddEffector(EndEffector endEffector)
    {
        if (assessor == null) assessor = FindObjectOfType<Assessor>();
        assessor.AddEffector(endEffector);
    }

    public void SetMapper(Mapper mapper)
    {
        if(mappers == null) mappers = new List<Mapper>();

        switch (academy.Mapping)
        {
            case Mappings.SteadyPointing:

                mapper.SetMapping(new Pointing());
                break;

            case Mappings.WristMove:
                mapper.SetMapping(new Pointing());
                break;

            case Mappings.LearnStaticMapping:
                mapper.SetMapping(new LearnStaticMapping());
                break;
        }
    }

    public void GetNeutralPosition()
    {
        _text.text = "Keep neutral position after the countdown\n6";
        var countdown = 6;
        for (var i = countdown; i >= 0; i--)
        {
            StartCoroutine(DisplayTextFor($"Keep neutral position after the countdown\n{i}", (countdown - i), i == 0));
        }
        StartCoroutine(DisplayTextFor("Done!", 7.0f, false));
        StartCoroutine(DisplayTextFor("", 9.0f, false));        
        StartCoroutine(assessor.FinaliseSavingPosition(10f));        
    }

    IEnumerator DisplayTextFor(string text, float time, bool measure)
    {
        yield return new WaitForSeconds(time);
        _text.text = text;
        savePosition = measure;
    }

}
