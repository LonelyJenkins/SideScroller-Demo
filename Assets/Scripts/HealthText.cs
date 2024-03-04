using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthText : MonoBehaviour
{
    public int healthValue;
    Text health;
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Text>();
        health.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        health.text = "TIMES HIT: " + healthValue;
    }
}
