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

    // Start is called before the first frame update
    void Start()
    {
        grandParentPose = transform.parent.parent;
        if (visualCue == null)
        {
            visualCue = grandParentPose.GetChild(0).Find(gameObject.name);
        }
        mat = visualCue.gameObject.GetComponent<Renderer>().material;
        pm = GameObject.Find("GlobalManager").GetComponent<SiFu_PoseManager>();
    }

    // Upon detecting another GameObject entering this GameObject's space, 
    // mark pose matched + change color of the visual cue on target. 
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            //Debug.Log("hit!");

            // change color of visual cues
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.green);

            // send signal to pose manager
            pm.setComponentMatch(gameObject.name, true);
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
