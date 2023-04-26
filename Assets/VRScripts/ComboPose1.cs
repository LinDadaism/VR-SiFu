using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboPose1 : SiFu_PoseAnime
{
    // Hands animation speed in units per second.
    public float speedLH = 1.0f;
    public float speedRH = 1.0f;

    // Quaternion endpts for animation
    public Quaternion startRotL;
    public Quaternion endRotL;
    public Quaternion startRotR;
    public Quaternion endRotR;

    [SerializeField]
    GameObject LH;
    [SerializeField]
    GameObject RH;

    void Awake()
    {
        if (LH)
        {
            startRotL = LH.transform.rotation;
        }
    }

    protected override void Update()
    {
        // Move character
        float bodyRatio = CalcLerpRatio(speedRH);
        transform.position = Vector3.Lerp(startMarker, endMarker, bodyRatio);

        // Animate hands
        if (LH)
        {
            float lhRatio = CalcLerpRatio(speedLH);
            LH.transform.rotation = Quaternion.Lerp(startRotL, endRotL, lhRatio);
        }
    }
}