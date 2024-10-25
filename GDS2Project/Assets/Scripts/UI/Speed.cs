using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speed : MonoBehaviour
{
    private Rigidbody rb;
    private TextMeshProUGUI speedText;
    private int speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        speedText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = (int)rb.velocity.magnitude * 5;
        speedText.text = speed.ToString();
        if (speed < 10)
        {
            speedText.text = "00" + speed.ToString();
        }
        else if (speed < 100)
        {
            speedText.text = "0" + speed.ToString();
        }
    }
}