using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarTester : MonoBehaviour
{
    public EnergyBar energyBar; 
    public float energyConsumeRate = 20f;
    private AbilityManager abilityManager; 

    private bool isDecreasingEnergy = false; 
    private bool isIncreasingEnergy = false;

    void Start()
    {
        abilityManager = GetComponent<AbilityManager>();
    }

    void Update()
    {
        // press "O", decreasing
        if (Input.GetKeyDown(KeyCode.O))
        {
            isDecreasingEnergy = true;
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            isDecreasingEnergy = false;
        }

        // press "P", increasing
        if (Input.GetKeyDown(KeyCode.P))
        {
            isIncreasingEnergy = true;
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            isIncreasingEnergy = false;
        }

        // Update energy
        if (isDecreasingEnergy)
        {
            float newEnergy = abilityManager.energySystem.GetCurrentEnergy() - energyConsumeRate * Time.deltaTime;
            energyBar.SetEnergy(newEnergy);
        }
        if (isIncreasingEnergy)
        {
            float newEnergy = abilityManager.energySystem.GetCurrentEnergy() + energyConsumeRate * Time.deltaTime;
            energyBar.SetEnergy(newEnergy);
        }
    }
}