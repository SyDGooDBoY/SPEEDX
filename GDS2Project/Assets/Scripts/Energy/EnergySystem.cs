using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    public GameObject fullscreenEffect; // boost fullscreen effect

    public float energyRecoveryRate = 20f; // Energy recovery rate per second
    public float energyDecreaseRate = 80f; // Energy decrease rate when stopped
    private float maxEnergy = AbilityManager.HIGH_THRESHOLD; // Maximum energy 
    private float currentEnergy; // Current energy for Get function

    private bool isRecovering = false; // Whether energy is currently recovering
    private float recoveryDelay = 1f; // Delay before starting to recover energy
    private float recoveryTimer = 0f; // Timer to track when to start recovery

    [Header("Boosting")]
    private bool isBoosting = false; 
    public float boostEnergyConsumptionRate = 40f; 

    void Start()
    {
        // Initialize start energy to stage 1
        currentEnergy = AbilityManager.LOW_THRESHOLD;
        Debug.Log("Current Energy: " + currentEnergy);

        if (fullscreenEffect != null)
        {
            fullscreenEffect.SetActive(false);
        }
    }

    void Update()
    {
        if (isBoosting)
        {
            ConsumeEnergyDuringBoost(); // boost consumption
        }else
        {
            if (isRecovering)
            {
                RecoverEnergy();
            }
            else
            {
                DecreaseEnergyOverTime(); // Gradually decrease energy when not recovering
            }
        }

        // Boost Logic
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // If it's not boosted and the energy is full, enter boost
            if (!isBoosting && currentEnergy >= maxEnergy)
            {
                EnterBoost();
            }
            // If already boosted, player can exit at any time
            else if (isBoosting)
            {
                ExitBoost();
            }
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Debug.Log("Current Energy: " + currentEnergy);
        //}
    }

    // enter boost state
    public void EnterBoost()
    {
        isBoosting = true;
        if (fullscreenEffect != null)
        {
            fullscreenEffect.SetActive(true); // active
        }
        Debug.Log("Boost activated.");
    }

    // exit boost state
    public void ExitBoost()
    {
        isBoosting = false;
        if (fullscreenEffect != null)
        {
            fullscreenEffect.SetActive(false); // deactive
        }
        Debug.Log("Boost deactivated.");
    }

    public bool IsBoosting()
    {
        return isBoosting; 
    }

    // Consume energy during boost
    private void ConsumeEnergyDuringBoost()
    {
        if (currentEnergy > 0)
        {
            currentEnergy -= boostEnergyConsumptionRate * Time.deltaTime;
            currentEnergy = Mathf.Max(currentEnergy, 0);

            // exit boost state when out of energy
            if (currentEnergy <= 0)
            {
                Debug.Log("Energy depleted, exiting boost.");
                ExitBoost(); 
            }
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

    // Gradually decrease energy when stopped
    public void DecreaseEnergyOverTime()
    {
        if (currentEnergy > 0)
        {
            currentEnergy -= energyDecreaseRate * Time.deltaTime;
            currentEnergy = Mathf.Max(currentEnergy, 0); // Ensure energy does not go below zero
        }
    }

    // Recover energy immediately (e.g. kill, sliding walls)
    public void RecoverEnergy(float amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
    }

    // Specific recovery through special actions
    public void RecoverEnergyThroughSpecialAction(float recoveryRate)
    {
        currentEnergy = Mathf.Min(currentEnergy + recoveryRate * Time.deltaTime, maxEnergy);
    }

    // Specific recovery through special actions (e.g., run, grappling)
    public void StartRecovery()
    {
        isRecovering = true;
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