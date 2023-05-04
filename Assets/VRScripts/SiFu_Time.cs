using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_Time : MonoBehaviour
{
    float max_time;
    public float start_scale;
    public float end_scale;
    float decrement_amount;
    float time_remaining;
    Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (time_remaining > 0) 
        {
            time_remaining -= Time.deltaTime;
            decrement_amount = (start_scale - end_scale) / (max_time / Time.deltaTime);
            transform.localScale -= new Vector3(0, decrement_amount, 0);
        }
        else 
        {
            //gameRunning = false;
            gameObject.SetActive(false);
        }
    }

    public void BurnIncense(float maxTime)
    {
        max_time = maxTime;
        time_remaining = max_time;
        decrement_amount = (start_scale - end_scale) / (max_time / Time.deltaTime);
        gameObject.SetActive(true);
        transform.localScale = initialScale;
    }
}
