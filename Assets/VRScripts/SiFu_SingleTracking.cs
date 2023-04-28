using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_SingleTracking : MonoBehaviour
{
    private Transform   grandParentPose;
    [SerializeField]
    private Transform   visualCue;
    private Material    mat;
    SiFu_PoseManager    pm;

    private bool        isMovingPose;
    public  float       percentMatch = 0.8f; // posing time period required to count as a match
    private float       startTime; // for detecting a constant collision over x amound of seconds

    // Start is called before the first frame update
    void Start()
    {
        isMovingPose = transform.parent.gameObject.GetComponent<Animator>() ? true : false;
        if (!isMovingPose)
        {
            grandParentPose = transform.parent.parent;
        }
        else
        {
            grandParentPose = transform.parent.parent.parent;

        }
        if (visualCue == null)
        {
            visualCue = grandParentPose.GetChild(0).Find(gameObject.name);
        }
        mat = visualCue.gameObject.GetComponent<Renderer>().material;
        pm = GameObject.Find("GlobalManager").GetComponent<SiFu_PoseManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            // change color of visual cues
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.green);

            if (!isMovingPose)
            {
                //Debug.Log("hit!");

                // send signal to pose manager
                pm.setComponentMatch(gameObject.name, true);
            }
            else
            {
                startTime = Time.time;
            }
        }
    }

    // Upon detecting another GameObject entering this GameObject's space, 
    // mark pose matched + change color of the visual cue on target. 
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GameController") && isMovingPose)
        {
            float timeElapsed = Time.time - startTime;

            // get current body part animation's length
            Animator anim = transform.parent.gameObject.GetComponent<Animator>(); // animator is attached on a parent obj
            if (anim != null)
            {
                float clipLength = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
                float animSpeed = anim.GetFloat("speed");
                clipLength = clipLength / animSpeed * percentMatch;

                if (timeElapsed > clipLength)
                {
                    Debug.Log("clip length: " + clipLength);
                    Debug.Log("component: " + gameObject.name);
                    // send signal to global manager
                    pm.setComponentMatch(gameObject.name, true);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            //Debug.Log("miss!");

            // change color of visual cues
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.red);

            // send signal to pose manager
            pm.setComponentMatch(gameObject.name, false);
        }
    }
}
