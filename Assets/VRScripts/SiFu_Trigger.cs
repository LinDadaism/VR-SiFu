using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class SiFu_Trigger : MonoBehaviour
    {
        public SteamVR_Input_Sources handType;

        public SteamVR_Action_Boolean grabPinchAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");

        public SteamVR_Action_Boolean grabGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

        public SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");

        public SteamVR_Action_Boolean uiInteractAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");

        public bool CheckGrabStarting()
        {
            //Debug.Log("grabState = " + grabGripAction.GetState(handType).ToString());

            if (grabPinchAction == null)
            {
                Debug.Log("NULL ACTION!");
            }
            return grabPinchAction.GetStateDown(handType); // || grabPinchAction.GetStateDown(handType)
        }

        public GrabTypes GetGrabStarting(GrabTypes explicitType = GrabTypes.None)
        {
            if (explicitType != GrabTypes.None)
            {
                //if (noSteamVRFallbackCamera)
                //{
                //    if (Input.GetMouseButtonDown(0))
                //        return explicitType;
                //    else
                //        return GrabTypes.None;
                //}

                if (explicitType == GrabTypes.Pinch && grabPinchAction.GetStateDown(handType))
                    return GrabTypes.Pinch;
                if (explicitType == GrabTypes.Grip && grabGripAction.GetStateDown(handType))
                    return GrabTypes.Grip;
            }
            else
            {
                //if (noSteamVRFallbackCamera)
                //{
                //    if (Input.GetMouseButtonDown(0))
                //        return GrabTypes.Grip;
                //    else
                //        return GrabTypes.None;
                //}

                if (grabPinchAction != null && grabPinchAction.GetStateDown(handType))
                    return GrabTypes.Pinch;
                if (grabGripAction != null && grabGripAction.GetStateDown(handType))
                    return GrabTypes.Grip;
            }

            return GrabTypes.None;
        }

        public GrabTypes GetGrabEnding(GrabTypes explicitType = GrabTypes.None)
        {
            if (explicitType != GrabTypes.None)
            {
                //if (noSteamVRFallbackCamera)
                //{
                //    if (Input.GetMouseButtonUp(0))
                //        return explicitType;
                //    else
                //        return GrabTypes.None;
                //}

                if (explicitType == GrabTypes.Pinch && grabPinchAction.GetStateUp(handType))
                    return GrabTypes.Pinch;
                if (explicitType == GrabTypes.Grip && grabGripAction.GetStateUp(handType))
                    return GrabTypes.Grip;
            }
            else
            {
                //if (noSteamVRFallbackCamera)
                //{
                //    if (Input.GetMouseButtonUp(0))
                //        return GrabTypes.Grip;
                //    else
                //        return GrabTypes.None;
                //}

                if (grabPinchAction.GetStateUp(handType))
                    return GrabTypes.Pinch;
                if (grabGripAction.GetStateUp(handType))
                    return GrabTypes.Grip;
            }

            return GrabTypes.None;
        }

        public void TriggerHapticPulse(ushort microSecondsDuration)
        {
            float seconds = (float)microSecondsDuration / 1000000f;
            hapticAction.Execute(0, seconds, 1f / seconds, 1, handType);
        }

        public void TriggerHapticPulse(float duration, float frequency, float amplitude)
        {
            hapticAction.Execute(0, duration, frequency, amplitude, handType);
        }
    }
}