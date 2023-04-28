using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* simple start/ pause system for testing */
public class SiFu_PauseButton3D : MonoBehaviour
{
    [SerializeField]
    SiFu_PoseManager pm;
    [SerializeField]
    GameObject startButton;

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("GameController") && pm.gameRunning)
        {
            Time.timeScale = 0;
            pm.gameRunning = false;

            // visual indication
            gameObject.SetActive(false);
            startButton.SetActive(true);
        }
    }
}
