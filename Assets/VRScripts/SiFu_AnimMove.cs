using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_AnimMove : MonoBehaviour
{
    public SiFu_SingleTracking tracking;

    public void OnClipBegin()
    {
        if (tracking != null)
        {
            tracking.OnEachLoopBegin();
        }
    }
}
