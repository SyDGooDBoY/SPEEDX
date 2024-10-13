using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [HideInInspector] public EnergySystem energySystem;

    //Energy thresholds
    public const float LOW_THRESHOLD = 200f;
    public const float MEDIUM_THRESHOLD = 300f;
    public const float HIGH_THRESHOLD = 400f;

    void Start()
    {
        energySystem = GetComponent<EnergySystem>();
    }

    // Determine the current energy stage
    public string GetCurrentEnergyStage()
    {
        float currentEnergy = energySystem.GetCurrentEnergy();

        if (currentEnergy <= LOW_THRESHOLD && currentEnergy >= 0f)
        {
            return "Low";
        }
        else if (currentEnergy <= MEDIUM_THRESHOLD)
        {
            return "Medium";
        }
        else if (currentEnergy <= HIGH_THRESHOLD)
        {
            return "High";
        }
        else
        {
            return "High"; // maybe higher threshold
        }
    }

    // Get a multiplier based on the current energy stage
    public float GetAbilityMultiplier()
    {
        //if (energySystem != null && energySystem.IsBoosting())
        //{
        //    string stage = GetCurrentEnergyStage();

        //    // different stage multiplier
        //    switch (stage)
        //    {
        //        case "High":
        //            return 1.8f;
        //        case "Medium":
        //            return 1.7f;
        //        default:
        //            return 1.5f; // Low
        //    }
        //}

        return 1.8f;

    }
}