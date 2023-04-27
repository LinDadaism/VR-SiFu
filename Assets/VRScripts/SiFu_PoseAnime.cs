using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_PoseAnime : MonoBehaviour
{
    // Transforms to act as start and end markers for the translation.
    public Vector3 startMarker;
    public Vector3 endMarker;

    // Movement speed in units per second.
    public float speed = 1.0f;

    // Time when the movement started.
    protected float startTime;

    // Total distance between the markers.
    protected float journeyLength;

    // body parts animation
    public List<Animator> animations = new List<Animator>();

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
        transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);

        //PauseAnim();
    }

    // stop animation after reaching arena
    void PauseAnim()
    {
        if (transform.position == endMarker)
        {
            foreach (Animator anim in animations)
            {
                anim.enabled = false;
            }
        }
    }
}
