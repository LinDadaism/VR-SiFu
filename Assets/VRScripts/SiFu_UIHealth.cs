using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SiFu_UIHealth : MonoBehaviour
{
    Text healthText;

    void Start()
    {
        healthText = GetComponent<Text>();
    }

    void Update()
    {
        healthText.text = SiFu_PoseManager.instance.health.ToString();
    }
}
