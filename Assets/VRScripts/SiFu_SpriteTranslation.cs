using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_SpriteTranslation : MonoBehaviour
{
    public Camera VRCamera;
    public float playerAreaRadius = 2.0f;
    Vector3 initPos;

    public void init()
    {
        initPos = transform.position;
        if (!VRCamera)
        {
            VRCamera = Camera.main;
            Debug.Log(VRCamera.name);
        }
    }

    // change from Update to LateUpdate so the camera is always updated first
    void LateUpdate()
    {
        if (VRCamera != null)
        {
            Vector3 camPos = VRCamera.transform.position;
            float xOffset = Mathf.Abs(camPos.x) > playerAreaRadius ?
                (camPos.x > 0.0f ? playerAreaRadius : -playerAreaRadius) : camPos.x;
            float zOffset = Mathf.Abs(camPos.z) > playerAreaRadius ?
                (camPos.z > 0.0f ? playerAreaRadius : -playerAreaRadius) : camPos.z;
            transform.position = initPos + new Vector3(xOffset, 0.0f, zOffset);
        }
    }
}
