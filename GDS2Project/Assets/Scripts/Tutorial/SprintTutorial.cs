using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintTutorial : MonoBehaviour
{
    public GameObject moveUI;
    public EnergySystem playerEnergySystem;

    void Start()
    {
        if (moveUI != null)
        {
            moveUI.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (moveUI != null)
            {
                if (playerEnergySystem.GetCurrentEnergy() < AbilityManager.HIGH_THRESHOLD 
                    && !playerEnergySystem.isBoosting)
                {
                    moveUI.SetActive(true);
                }
                else
                {
                    moveUI.SetActive(false);
                }

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
