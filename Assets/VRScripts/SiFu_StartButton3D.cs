using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_StartButton3D : MonoBehaviour
{
    public ParticleSystem system;

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("GameController") && !SiFu_PoseManager.instance.gameRunning)
        {
            SiFu_PoseManager.instance.StartGame();

            // ui and sound effects
            GetComponent<AudioSource>().Play();
            system.Play(/*includeChildren*/true);
        }
    }
}
