using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_PickUpWeapon : MonoBehaviour
{
    public Valve.VR.InteractionSystem.SiFu_Trigger trigger;

    bool inBox = false;
    bool holding = false;

    SpriteRenderer rend;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(trigger.CheckGrabStarting())
        {
            if(inBox && !holding)
            {
                PickUp();
            }
            else if(holding)
            {
                Release();
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("GameController"))
        {
            inBox = true;
            rend.color = Color.white;
            Debug.Log("Player near weapon");
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("GameController"))
        {
            inBox = false;
            rend.color = Color.grey;
            Debug.Log("Player far weapon");
        }
    }

    void PickUp()
    {
        holding = true;

        Debug.Log("Try To PickUp");
        // TODO
        rend.enabled = false;

    }

    void Release()
    {
        holding = false;
        Debug.Log("Try To PickUp");
        rend.enabled = true;
    }
}
