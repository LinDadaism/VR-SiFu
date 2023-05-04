using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_StartButton3D : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("GameController") && !SiFu_PoseManager.instance.gameRunning)
        {
            GetComponent<AudioSource>().Play();
            SiFu_PoseManager.instance.StartGame();
        }
    }
}
