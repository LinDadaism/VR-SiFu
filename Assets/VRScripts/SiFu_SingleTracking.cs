using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// alias
using SiFu_Trigger = Valve.VR.InteractionSystem.SiFu_Trigger;

public class SiFu_SingleTracking : MonoBehaviour
{
    private Transform   grandParentPose;
    [SerializeField]
    private Transform   visualCue;
    private Material    mat;
    SiFu_PoseManager    pm;
    Animator            anim;

    private bool        isMovingPose;
    private float       percentMatch = 0.7f; // posing time period required to count as a match
    private float       triggerStartTime; // for detecting a constant collision over x amound of seconds
    private float       triggerLastTime;
    private float       triggerAccumulateTime;

    private bool        hasWeapon;
    public Transform   weapon;
    // public float weaponRotBasis = 0.0f; // represent the z-axis rotation
    public  Transform   weaponVisualCue;
    private float       initWeaponRotZ;
    public  float       rotOffset = 20.0f; // in degrees
    public  int         weaponType = 0;
    private float       clipMatchTime;

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
            anim = transform.parent.gameObject.GetComponent<Animator>(); // animator is attached on a parent obj
            float clipLength = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            float animSpeed = anim.GetFloat("speed");
            clipMatchTime = (clipLength / animSpeed) * percentMatch; // the time period a player needs to stay matching
        }

        hasWeapon = transform.childCount > 0 ? transform.GetChild(0).gameObject.name == "Weapon" : false;
        if (hasWeapon)
        {
            weapon = transform.GetChild(0);
            initWeaponRotZ = weapon.rotation.eulerAngles.z;
            // weaponVisualCue = grandParentPose.GetChild(0).Find(gameObject.name).GetChild(0);
            SiFu_PoseManager.instance.LookForWeapon(weaponType, weapon.gameObject);
            // weapon.rotation = Quaternion.Euler(0, 0, initWeaponRotZ + 90);
        }

        // below can only find the first layer of children visual cue;
        // if combo/weapon, need to manually assign visual cue in prefab
        if (visualCue == null)
        {
            visualCue = grandParentPose.GetChild(0).Find(gameObject.name);
        }
        mat = visualCue.gameObject.GetComponent<Renderer>().material;
        pm = SiFu_PoseManager.instance;

        triggerStartTime = Time.time;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            // change color of visual cues
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.green);

            if (!isMovingPose && !hasWeapon)
            {
                //Debug.Log("hit!");
                OnComponentMatch();
            }
            if (isMovingPose)
            {
                // triggerStartTime = Time.time;
                triggerLastTime = Time.time;
            }
            if (hasWeapon)
            {
                mat.SetColor("_EmissionColor", new Color(1.0f, 0.64f, 0)); //orange
            }
        }
    }

    // cap angle in between 0-360 degrees
    float ConvertAngleDeg(float angle)
    {
        while (!(-180.0f < angle && angle < 180.0f))
        {
            if (angle > 180.0f) angle -= 360.0f;
            if (angle < -180.0f) angle += 360.0f;
        }
        return angle;
    }

    // Upon detecting another GameObject entering this GameObject's space, 
    // mark pose matched + change color of the visual cue on target. 
    void OnTriggerStay(Collider other)
    {
        // haptic feedback
        int duration = 500000;
        SiFu_Trigger trigger = other.transform.gameObject.GetComponent<SiFu_Trigger>();
        if (trigger) trigger.TriggerHapticPulse((ushort)duration);

        // moving pose match logic
        if (other.CompareTag("GameController") && isMovingPose)
        {

            // float timeElapsed = Time.time - triggerStartTime;
            triggerAccumulateTime += Time.time - triggerLastTime;
            triggerLastTime = Time.time;

            // get current body part animation's length
            if (anim != null)
            {
                if (triggerAccumulateTime > clipMatchTime)
                {
                    // Debug.Log("movingPose passed with ratio " + (triggerAccumulateTime / clipMatchTime).ToString());
                    //Debug.Log("clip length: " + clipLength);
                    //Debug.Log("component: " + gameObject.name);
                    OnComponentMatch();
                }
            }
        }

        // weapon pose match logic
        if (hasWeapon && weaponType == SiFu_PoseManager.instance.holdingWeaponType)
        {
            Debug.Log("weapon right, check rotation");
            // sync weapon rotation with controller's
            weapon.localRotation = Quaternion.Euler(0, 0, ConvertAngleDeg(other.transform.rotation.eulerAngles.z + 90));
            weaponVisualCue.localRotation = weapon.localRotation;

            float currZ = weapon.localRotation.eulerAngles.z;
            // float currZ = other.transform.rotation.eulerAngles.z;
            currZ = ConvertAngleDeg(currZ);
            
            // both position and rotation match
            if (currZ > initWeaponRotZ - rotOffset && 
                currZ < initWeaponRotZ + rotOffset)
            {
                mat.SetColor("_EmissionColor", Color.green);
                OnComponentMatch();
            }
            // position matched but not rotation
            else
            {
                mat.SetColor("_EmissionColor", new Color(1.0f, 0.64f, 0));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GameController"))
        {
            //Debug.Log("miss!");
            OnComponentMatch(false);

            // change color of visual cues
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.red);
        }
    }

    void OnComponentMatch(bool isMatch = true)
    {
        if (pm) pm.setComponentMatch(gameObject.name, isMatch);
    }

    public void OnEachLoopBegin()
    {
        
        Debug.Log("clipMatchTime : " + clipMatchTime.ToString());
        Debug.Log("actual duraction : " + (Time.time - triggerStartTime).ToString());
        Debug.Log("triggerAccumulateTime : " + triggerAccumulateTime.ToString());
        
        triggerStartTime = Time.time;
        triggerAccumulateTime = 0.0f;
    }
}
