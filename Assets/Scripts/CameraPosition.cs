using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using Valve.VR;

namespace DefaultNamespace
{
    /// <summary>
    /// This component simplifies the use of Pose actions. Adding it to a gameobject will auto set that transform's position and rotation every update to match the pose.
    /// Advanced velocity estimation is handled through a buffer of the last 30 updates.
    /// </summary>
    public class CameraPosition : MonoBehaviour
    {
        public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

        [Tooltip("The device this action should apply to. Any if the action is not device specific.")]
        public SteamVR_Input_Sources inputSource;

        [Tooltip("If not set, relative to parent")]
        public Transform origin;


        [Tooltip("Can be disabled to stop broadcasting bound device status changes")]
        public bool broadcastDeviceChanges = true;

        private int deviceIndex = -1;

        private readonly SteamVR_HistoryBuffer historyBuffer = new SteamVR_HistoryBuffer(30);
        

        protected virtual void Start()
        {
            if (poseAction == null)
            {
                Debug.LogError("<b>[SteamVR]</b> No pose action set for this component");
                return;
            }

            CheckDeviceIndex();

            if (origin == null)
                origin = this.transform.parent;
            
        }

        protected virtual void OnEnable()
        {
            SteamVR.Initialize();

            if (poseAction != null)
            {
                poseAction[inputSource].onUpdate += SteamVR_Behaviour_Pose_OnUpdate;
            }
        }

        protected virtual void OnDisable()
        {
            if (poseAction != null)
            {
                poseAction[inputSource].onUpdate -= SteamVR_Behaviour_Pose_OnUpdate;
            }

            historyBuffer.Clear();
        }

        private void SteamVR_Behaviour_Pose_OnUpdate(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
        {
            UpdateHistoryBuffer();

            UpdateTransform();

        }
        protected virtual void UpdateTransform()
        {
            CheckDeviceIndex();

            if (origin != null)
            {
                var y = transform.position.y;
                var newPosition = origin.transform.TransformPoint(poseAction[inputSource].localPosition);
                newPosition.y = y;
                transform.position = newPosition;
            }
            else
            {
                transform.localPosition = poseAction[inputSource].localPosition;
                transform.localRotation = poseAction[inputSource].localRotation;
            }
        }


        protected virtual void CheckDeviceIndex()
        {
            if (poseAction[inputSource].active && poseAction[inputSource].deviceIsConnected)
            {
                int currentDeviceIndex = (int)poseAction[inputSource].trackedDeviceIndex;

                if (deviceIndex != currentDeviceIndex)
                {
                    deviceIndex = currentDeviceIndex;

                    if (broadcastDeviceChanges)
                    {
                        this.gameObject.BroadcastMessage("SetInputSource", inputSource, SendMessageOptions.DontRequireReceiver);
                        this.gameObject.BroadcastMessage("SetDeviceIndex", deviceIndex, SendMessageOptions.DontRequireReceiver);
                    }

                }
            }
        }

        /// <summary>
        /// Returns the device index for the device bound to the pose. 
        /// </summary>
        public int GetDeviceIndex()
        {
            if (deviceIndex == -1)
                CheckDeviceIndex();

            return deviceIndex;
        }

        /// <summary>Returns the current velocity of the pose (as of the last update)</summary>
        public Vector3 GetVelocity()
        {
            return poseAction[inputSource].velocity;
        }

        /// <summary>Returns the current angular velocity of the pose (as of the last update)</summary>
        public Vector3 GetAngularVelocity()
        {
            return poseAction[inputSource].angularVelocity;
        }

        /// <summary>Returns the velocities of the pose at the time specified. Can predict in the future or return past values.</summary>
        public bool GetVelocitiesAtTimeOffset(float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity)
        {
            return poseAction[inputSource].GetVelocitiesAtTimeOffset(secondsFromNow, out velocity, out angularVelocity);
        }

        /// <summary>Uses previously recorded values to find the peak speed of the pose and returns the corresponding velocity and angular velocity</summary>
        public void GetEstimatedPeakVelocities(out Vector3 velocity, out Vector3 angularVelocity)
        {
            int top = historyBuffer.GetTopVelocity(10, 1);

            historyBuffer.GetAverageVelocities(out velocity, out angularVelocity, 2, top);
        }

        protected int lastFrameUpdated;
        protected void UpdateHistoryBuffer()
        {
            int currentFrame = Time.frameCount;
            if (lastFrameUpdated != currentFrame)
            {
                historyBuffer.Update(poseAction[inputSource].localPosition, poseAction[inputSource].localRotation, poseAction[inputSource].velocity, poseAction[inputSource].angularVelocity);
                lastFrameUpdated = currentFrame;
            }
        }





    }
}