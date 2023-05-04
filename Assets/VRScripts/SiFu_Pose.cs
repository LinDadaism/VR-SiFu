using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_Pose : MonoBehaviour
{
    // Movement speed in units per second.
    public float speed = 1.0f;

    // if waitTime has passed, missed a pose and reduce player's health
    public float waitTime = 8.0f;

    // Time when the movement started.
    protected float startTime;

    // Total distance between the markers.
    protected float journeyLength;

    // body parts animation
    public List<Animator> animations = new List<Animator>();

    public Transform cueTransform;

    Vector3 startMarker;
    Vector3 endMarker;

    protected float CalcLerpRatio(float speed)
    {
        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;
        return fractionOfJourney;
    }
    
    void Awake()
    {
        if (gameObject.tag == "StaticPose")
        {
            waitTime = 10.0f;
        }
        if (gameObject.tag == "ComboPose")
        {
            waitTime = 25.0f;
        }
        if (gameObject.tag == "WeaponPose")
        {
            waitTime = 20000.0f;
        }
    }

    void Start()
    {
        // Keep a note of the time the movement started.
        startTime = Time.time;

        startMarker = SiFu_PoseManager.instance.cueStartMarker;
        endMarker = SiFu_PoseManager.instance.cueEndMarker;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(startMarker, endMarker);

        if(gameObject.tag == "WeaponPose")
        {
            // TODO: Set PickUp Weapon Prefab 
        }
        Debug.Log("wait time: " + waitTime);
    }

    // Move to the target end position.
    void Update()
    {

        float fractionOfJourney = CalcLerpRatio(speed);
        // Set our position as a fraction of the distance between the markers.
        cueTransform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);

        //PauseAnim();
        if (Time.time - startTime > waitTime)
        {
            SiFu_PoseManager.instance.HitPlayer();
        }
    }

    // stop animation after reaching arena
    void PauseAnim()
    {
        if (cueTransform.position == endMarker)
        {
            foreach (Animator anim in animations)
            {
                anim.enabled = false;
            }
        }
    }

    public void SetScale(float ratio)
    {
        // adjust the scale accoridng to player's height
        Transform target = transform.Find("Panda2DTarget");
        if(target == null) {
            Debug.LogError("Can't find Panda2DTarget");
            return;
        }
        List<Transform> foots = new();
        List<Transform> hands = new();
        for(int i = 0; i < 4; i++)
        {
            Transform targetChild = target.GetChild(i);
            if(targetChild.gameObject.name.Contains("Foot"))
            {
                foots.Add(targetChild);
            }
            else
            {
                hands.Add(targetChild);
            }
        }

        // TODO
        //transform.position = transform.position.y 


        SiFu_SpriteTranslation tranComp = target.GetComponent<SiFu_SpriteTranslation>();
        if(tranComp == null)
        {
            Debug.LogError("Can't find SiFu_SpriteTranslation on Panda2DTarget");
        }
        else
        {
            tranComp.init();
        }
    }

    public void Die()
    {
        // any vfx effect
        //Instantiate(deathExplosion, gameObject.transform.position, Quaternion.AngleAxis(-90, Vector3.right));

        //GameObject obj = GameObject.Find("GlobalObject");
        //GlobalManager m = obj.GetComponent<GlobalManager>();
        //m.score += pointValue;

        // play sound effect
        //AudioSource[] sounds = obj.GetComponents<AudioSource>();
        //foreach (AudioSource sound in sounds)
        //{
        //    if (sound.clip.name == "bop")
        //    {
        //        sound.Play();
        //    }
        //}

        Debug.Log("pose clear");

        // Destroy removes the gameObject from the scene and
        // marks it for garbage collection
        Destroy(gameObject);
    }
}
