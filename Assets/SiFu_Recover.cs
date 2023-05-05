using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_Recover : MonoBehaviour
{
    public int amount;

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("GameController"))
        {
            SiFu_PoseManager.instance.health += amount;
            gameObject.SetActive(false);
            // ui and sound effects
        }
    }
}
