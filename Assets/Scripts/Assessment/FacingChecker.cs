﻿using System;
using System.Collections;
using System.Globalization;
using Rokoko.Smartsuit;
using UnityEngine;

namespace Assessment
{

    public class FacingChecker : MonoBehaviour
    {
        
        public Camera Camera;
        public GameObject StartArea;
        public GameObject LeftHand;
        public GameObject RightHand;

        private bool _startedCounting = false;
        private bool _active = false;
        private int _countdown;
        private int _countdownStart = 2;
        private Action _callback;
        private bool _inPosition = false;
        private Vector3 _forward;
        private Vector3 _up;
        private GameObject _vlight;
        
        private void Start()
        {
            _forward = StartArea.transform.transform.forward;
            _up = StartArea.transform.transform.up;
            _countdown = _countdownStart;
            _vlight = StartArea.transform.GetChild(0).gameObject;
            _vlight.SetActive(false); 
        }

        private void Update()
        {
            if (_active)
            {
                    if (!InTheArea())
                    {
                        UIHandler.startDisplay("Please go to the start area");
                    _vlight.SetActive(true);
                    _countdown = _countdownStart;
                    }
                    else
                    {
                    _vlight.SetActive(false);
                    if (!FacingForward())
                        {
                            UIHandler.startDisplay("Please turn to the playing area");
                            _countdown = _countdownStart;
                        }
                        else 
                        if (!HandDown(LeftHand) || !HandDown(RightHand))
                        {
                            UIHandler.startDisplay("Please hold your hands down");
                            _countdown = _countdownStart;
                        }
                        else
                            {
                                if (!_startedCounting)
                                {
                                    StartCoroutine(nameof(LoseTime));
                                    _startedCounting = true;
                                    _inPosition = true;
                                }
                                UIHandler.startDisplay("" + _countdown);
                            }
                        }
                if (_inPosition && _countdown <= 0)
                    StopCountdown();
            }
            else
            {
                _vlight.SetActive(false);
            }
        }

        public void ActivateCountdown(Action callback, int countdownStart)
        {
            _countdownStart = countdownStart;
            _countdown = countdownStart;
            _callback = callback;
            _active = true;
            _startedCounting = false;
            _inPosition = false;
        }

        public void StopCountdown()
        {
            _callback();
            _active = false;
            _startedCounting = false;
            UIHandler.stopDisplaying();
        }


        private bool InTheArea()
        {
            return StartArea.GetComponent<Collider>().bounds.Contains(Camera.transform.position);
        }

        private bool FacingForward()
        {
            return Vector3.Angle(_forward, Camera.transform.forward) < 20;
        }

        private bool HandDown(GameObject hand)
        {
            var angle = Vector3.Angle(_up, hand.transform.up);
            return Vector3.Angle(_up, hand.transform.up) < 35;
        }


        IEnumerator LoseTime()
        {
            while (_countdown >= 0)
            {
                yield return new WaitForSeconds(1);
                _countdown--;
            }
        }

    }
}