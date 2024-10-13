using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ShowUIOnTrigger : MonoBehaviour
{
    public GameObject moveUI;   

    void Start()
    {
        if (moveUI != null)
        {
            moveUI.SetActive(false);  
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (moveUI != null)
            {
                moveUI.SetActive(true);  
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (moveUI != null)
            {
                moveUI.SetActive(false);  
            }
        }
    }
}

