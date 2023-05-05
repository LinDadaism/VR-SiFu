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
            // UI and sound effects
            AudioSource source = GetComponent<AudioSource>();
            source.Play();
            system.Play(true);

            Debug.Log("Controller Hit Start Button");
            StartCoroutine(StartGameLater());
        }
    }

    IEnumerator StartGameLater()
    {
        yield return new WaitForSeconds(2.0f);
        SiFu_PoseManager.instance.StartGame(level);
    }
}
