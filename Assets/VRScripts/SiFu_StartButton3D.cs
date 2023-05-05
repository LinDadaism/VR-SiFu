using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_StartButton3D : MonoBehaviour
{
    public ParticleSystem system;

    public int level = 0;

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hit Start Button");
        if (other.collider.CompareTag("GameController") && !SiFu_PoseManager.instance.gameRunning)
        {
            Debug.Log("Controller Hit Start Button");
            SiFu_PoseManager.instance.StartGame(level);

            // ui and sound effects
            GetComponent<AudioSource>().Play();
            system.Play(/*includeChildren*/true);
        }
    }
}
