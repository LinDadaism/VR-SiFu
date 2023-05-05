using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_PickUpWeapon : MonoBehaviour
{
    Valve.VR.InteractionSystem.SiFu_Trigger trigger;
   
    // public GameObject handWeapon;

    bool inBox = false;
    public bool holding = false;

    public int weaponType = 1;

    SpriteRenderer rend;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        trigger = Valve.VR.InteractionSystem.SiFu_Trigger.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(trigger.CheckGrabStarting())
        {
            Debug.Log("Trigger in PickUp");
            if(inBox && !holding && SiFu_PoseManager.instance.targetWeaponType == weaponType)
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
            rend.color = new Color(171f/256f, 171f / 256f, 171f / 256f, 230f / 256f);
            Debug.Log("Player far weapon");
        }
    }

    void PickUp()
    {
        holding = true;
        
        rend.enabled = false;

        Debug.Log("Try To PickUp");
        // handWeapon.SetActive(true);
        SiFu_PoseManager.instance.SetWeapon(weaponType);
    }

    public void Release()
    {
        holding = false;
        Debug.Log("Try To Release");

        rend.enabled = true;
        // handWeapon.SetActive(false);
        // SiFu_PoseManager.instance.SetWeapon(0);
    }
}
