﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rokoko.Smartsuit;

public class SmartsuitSyncWithHMD : MonoBehaviour {

    public SmartsuitActor actor;
    public Transform HMD;

    private Transform characterEyes;
    private Transform characterRoot;

    public float rotationWeight = 1;
    public float positionWeight = 1;

	// Use this for initialization
	void Start () {

        if (actor==null)
        {
            actor = GetComponent<SmartsuitActor>();
        }
        if (actor && !actor.ActorStarted)
        {
            actor.OnActorStart += LateStart;
        }
        else
        {
            LateStart();
        }

    }

    void LateStart ()
    {
        if (characterRoot == null)
        {
            if (actor)
            {
                characterRoot = actor.transform;
            }
            else
            {
                Debug.Log("Smartsuit is missing");
            }

        }
        if (characterEyes == null)
        {
            Transform head = actor.Bone(HumanBodyBones.Head);
            if (head)
            {
                characterEyes = head.Find("Eyes");
                if (characterEyes == null)
                {
                    Debug.Log("Creating Eyes");
                    characterEyes = new GameObject("Eyes").transform;
                    characterEyes.parent = head;
                    characterEyes.rotation = characterRoot.rotation;
                    characterEyes.position = head.position + new Vector3(0, .12f, .1f);
                }

            }
            else
            {
                Debug.Log("Smartsuit have no head");
                this.enabled = false;
                return;
            }
        }

        if (!characterEyes.IsChildOf(characterRoot))
        {
            Debug.Log("Character Eyes is not child of Character Root");
            this.enabled = false;
            return;
        }

        characterRoot.rotation = Quaternion.identity;

        if (Vector3.Dot(HMD.forward, characterRoot.forward)<0)
        {
            characterRoot.rotation *= Quaternion.Euler(0,180,0);
        }

    }
	
	// Update is called once per frame
	void Update () {
        Sync();        
	}

    void LateUpdate()
    {
         
    }


    public void Sync()
    {

        Vector3 forwardaxis;
        Vector3 headfw = characterEyes.forward;
        Vector3 headup = characterEyes.up;
        Vector3 rootup = Vector3.up;
        float fw_dot = Mathf.Abs(Vector3.Dot(rootup, headup));
        float up_dot = Mathf.Abs(Vector3.Dot(rootup, headfw));
        if (fw_dot > up_dot)
        {
            forwardaxis = Vector3.forward;
        }
        else
        {
            forwardaxis = Vector3.up;
        }        
        Vector3 hmdfw = HMD.rotation* forwardaxis;
        Vector3 eyefw = characterEyes.rotation * forwardaxis;

        //Vector3.OrthoNormalize(ref rootup, ref hmdfw);
        //Vector3.OrthoNormalize(ref rootup, ref eyefw);
        hmdfw.y = 0;
        eyefw.y = 0;

        Quaternion rotationError = Quaternion.FromToRotation(eyefw, hmdfw);

        characterRoot.rotation = Quaternion.Lerp(characterRoot.rotation, rotationError*characterRoot.rotation, rotationWeight);

        var shift = headfw * 0.05f;

        Vector3 positionError = HMD.position - characterEyes.position - shift;
        Vector3 cp = characterRoot.position;

        characterRoot.position += positionError * Mathf.Clamp01(positionWeight);

    }

}
