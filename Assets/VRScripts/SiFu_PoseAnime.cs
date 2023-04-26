using System.Collections;
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

    protected float CalcLerpRatio(float speed)
    {
        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;
        return fractionOfJourney;
    }

    virtual protected void Start()
    {
        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(startMarker, endMarker);
    }

    // Move to the target end position.
    virtual protected void Update()
    {

        float fractionOfJourney = CalcLerpRatio(speed);
        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
    }
}
