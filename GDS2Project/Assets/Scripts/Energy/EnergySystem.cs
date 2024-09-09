using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    public float energyRecoveryRate = 5f; // Energy recovery rate per second
    private float maxEnergy = AbilityManager.HIGH_THRESHOLD; // Maximum energy 
    private float currentEnergy; // Current energy for Get function

    private bool isRecovering = false; // Whether energy is currently recovering
    private float recoveryDelay = 1f; // Delay before starting to recover energy
    private float recoveryTimer = 0f; // Timer to track when to start recovery

    void Start()
    {
        // Initialize start energy to stage 1
        currentEnergy = AbilityManager.LOW_THRESHOLD;
        Debug.Log("Current Energy: " + currentEnergy);
    }

    void Update()
    {
        if (isRecovering)
        {
            RecoverEnergy();
        }

        // just for Test
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseEnergy(20f);
            Debug.Log("Current Energy: " + currentEnergy);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Current Energy: " + currentEnergy);
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            RecoverEnergy(20f);
            Debug.Log("Current Energy: " + currentEnergy);
        }
    }

    // Consume energy
    public bool UseEnergy(float amount)
    {
        if (currentEnergy == 0)
        {
            return false;
        }

        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
        }
        else // 0 < currentEnergy < amount
        {
            SetEnergy(0f);
        }
        StopRecovery(); // Stop energy recovery on energy usage
        return true;
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

    // Stop energy recovery and start a delay timer
    public void StopRecovery()
    {
        isRecovering = false;
        recoveryTimer = 0f; // Reset the recovery timer
    }

    // Call this method when player stops moving
    public void TryStartRecovery()
    {
        recoveryTimer += Time.deltaTime;
        if (recoveryTimer >= recoveryDelay)
        {
            isRecovering = true; // Start recovery after the delay
        }
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

    // Consume Energy based on delta time
    public void ConsumeEnergyOverTime(float consumptionRate)
    {
        float energyToConsume = consumptionRate * Time.deltaTime;
        UseEnergy(energyToConsume);
    }
}