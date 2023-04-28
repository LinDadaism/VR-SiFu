using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* simple start/ pause system for testing */
public class SiFu_StartButton3D : MonoBehaviour
{
    [SerializeField]
    SiFu_PoseManager pm;
    [SerializeField]
    GameObject pauseButton;

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("GameController") && !pm.gameRunning)
        {
            Time.timeScale = 1;
            pm.gameRunning = true;

            // visual indication
            gameObject.SetActive(false);
            pauseButton.SetActive(true);
        }
    }
}
