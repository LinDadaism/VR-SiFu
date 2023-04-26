using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_Pose : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScale(float scale)
    {
        //
    }

    public void Die()
    {
        // any vfx effect
        //Instantiate(deathExplosion, gameObject.transform.position, Quaternion.AngleAxis(-90, Vector3.right));

        //GameObject obj = GameObject.Find("GlobalObject");
        //GlobalManager m = obj.GetComponent<GlobalManager>();
        //m.score += pointValue;

        // play sound effect
        //AudioSource[] sounds = obj.GetComponents<AudioSource>();
        //foreach (AudioSource sound in sounds)
        //{
        //    if (sound.clip.name == "bop")
        //    {
        //        sound.Play();
        //    }
        //}

        Debug.Log("pose clear");

        // Destroy removes the gameObject from the scene and
        // marks it for garbage collection
        Destroy(gameObject);
    }
}
