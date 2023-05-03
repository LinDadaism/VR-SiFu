using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_Pose : MonoBehaviour
{
    // Transforms to act as start and end markers for the translation.
    public Vector3 startMarker;
    public Vector3 endMarker;

    // Movement speed in units per second.
    public float speed = 1.0f;

    // if waitTime has passed, clear and reduce player's health
    public float waitTime = 8.0f;

    // Time when the movement started.
    protected float startTime;

    // Total distance between the markers.
    protected float journeyLength;

    // body parts animation
    public List<Animator> animations = new List<Animator>();

    public Transform cueTransform;

    protected float CalcLerpRatio(float speed)
    {
        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;
        return fractionOfJourney;
    }

    void Start()
    {
        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(startMarker, endMarker);
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
        // TODO: adjust the scale accoridng to player's height
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
