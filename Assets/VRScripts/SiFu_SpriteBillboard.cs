using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_SpriteBillboard : MonoBehaviour
{
    [SerializeField]
    bool freezeXZAxis = true;

    public Camera VRCamera;

    // change from Update to LateUpdate so the camera is always updated first
    void LateUpdate()
    {
        if (VRCamera != null)
        {
            if (freezeXZAxis)
            {
                transform.rotation = Quaternion.Euler(0f, VRCamera.transform.rotation.eulerAngles.y, 0f);
            }
            else
            {
                transform.rotation = VRCamera.transform.rotation;
            }
        }
    }
}
