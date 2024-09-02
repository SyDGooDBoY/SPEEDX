using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    public float energyRecoveryRate = 5f; // Energy recovery rate per second
    private float maxEnergy = AbilityManager.HIGH_THRESHOLD; // Maximum energy 
    private float currentEnergy; // Current energy for Get function

    private bool isRecovering = true; // Whether energy is currently recovering

    void Start()
    {
        // Initialize start energy to stage 1
        currentEnergy = AbilityManager.LOW_THRESHOLD;
    }

    void Update()
    {
        if (isRecovering)
        {
            RecoverEnergy();
        }
    }

    // Consume energy
    public bool UseEnergy(float amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            isRecovering = false;
            Invoke("StartRecovery", 1f); // Stop recovery for 1 seconds before starting again
            return true;
        }
        else
        {
            return false; // Not enough energy to perform action
        }
    }

    // Recover energy over time
    private void RecoverEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRecoveryRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy); // Ensure energy less than the maximum
        }
    }

    // Recover energy immediately (e.g. kill, sliding walls)
    public void RecoverEnergy(float amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
    }

    // Start recovering energy
    private void StartRecovery()
    {
        isRecovering = true;
    }

    // Get current energy level
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    // Just used to test
    public void SetEnergy(float energy)
    {
        currentEnergy = Mathf.Min(energy, maxEnergy);
    }
}
