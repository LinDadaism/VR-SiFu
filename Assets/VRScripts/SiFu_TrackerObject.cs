//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: For controlling in-game objects with tracked devices.
//
//=============================================================================

using UnityEngine;
using Valve.VR;
using System;

namespace Valve.VR
{
    public class SiFu_TrackerObject : MonoBehaviour
    {
        public enum EIndex
        {
            None = -1,
            Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
            Device1,
            Device2,
            Device3,
            Device4,
            Device5,
            Device6,
            Device7,
            Device8,
            Device9,
            Device10,
            Device11,
            Device12,
            Device13,
            Device14,
            Device15,
            Device16
        }

        public int tracker_index;

        int tracker_count = 0;

        public EIndex index;

        [Tooltip("If not set, relative to parent")]
        public Transform origin;

        public bool isValid { get; private set; }

        private void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            if (index == EIndex.None)
                return;

            var i = (int)index;

            isValid = false;
            if (poses.Length <= i)
                return;

            if (!poses[i].bDeviceIsConnected)
                return;

            if (!poses[i].bPoseIsValid)
                return;

            isValid = true;

            var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);

            if (origin != null)
            {
                transform.position = origin.transform.TransformPoint(pose.pos);
                transform.rotation = origin.rotation * pose.rot;
            }
            else
            {
                transform.localPosition = pose.pos;
                transform.localRotation = pose.rot;
            }
        }

        SteamVR_Events.Action newPosesAction;

        SiFu_TrackerObject()
        {
            newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
        }

        private void Awake()
        {
            OnEnable();
            //uint index = 0;
            var error = ETrackedPropertyError.TrackedProp_Success;
            for (uint i = 0; i < 16; i++)
            {
                var result = new System.Text.StringBuilder((int)64);
                OpenVR.System.GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_RenderModelName_String, result, 64, ref error);
                //Debug.Log(result.ToString());
                //Debug.Log(i.ToString());
                if (result.ToString().Contains("tracker"))
                {
                    if(tracker_count == tracker_index)
                    {
                        //index = i;
                        SetDeviceIndex((int)i);
                        Debug.Log(Enum.GetName(typeof(EIndex), index));
                        break;
                    }
                    else
                    {
                        tracker_count++;
                    }
                }
            }
        }

        void OnEnable()
        {
            var render = SteamVR_Render.instance;
            if (render == null)
            {
                enabled = false;
                return;
            }

            newPosesAction.enabled = true;
        }

        void OnDisable()
        {
            newPosesAction.enabled = false;
            isValid = false;
        }

        public void SetDeviceIndex(int index)
        {
            if (System.Enum.IsDefined(typeof(EIndex), index))
                this.index = (EIndex)index;
        }

        void Update()
        {
            // print out tracker's global location
            //Debug.Log("tracker pos: " + transform.position);
        }
    }
}