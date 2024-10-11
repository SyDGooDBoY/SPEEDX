using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class GradientColor : MonoBehaviour
{
    public Slider slider;
    // public Gradient gradient;
    public Image fill;

    // public Vector3 offset = new Vector3(0, 15, 0);
    // public CookingFireController Cooking;

    public void Start()
    {
        gameObject.SetActive(false);
    }
    
    public void EnableUI()
    {
        gameObject.SetActive(true);
    }

    public void DisableUI()
    {
        gameObject.SetActive(false);
    }

    public void SetMaxValue(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;
       
    }

    public void UpdateValue(float currentValue)
    {
        slider.value = currentValue;
    }


    public void SetTime(int time)
    {
    }
}