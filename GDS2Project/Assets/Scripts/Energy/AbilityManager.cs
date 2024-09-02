using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private EnergySystem energySystem;

    //Energy thresholds
    private const float LOW_THRESHOLD = 100f;
    private const float MEDIUM_THRESHOLD = 200f;
    private const float HIGH_THRESHOLD = 400f;

    //Speed and Jump Height based on energy stages
    private Dictionary<string, float> speedByStage = new Dictionary<string, float>
    {
        { "Low", 3f },
        { "Medium", 5f },
        { "High", 7f }
    };

    private Dictionary<string, float> jumpHeightByStage = new Dictionary<string, float>
    {
        { "Low", 5f },
        { "Medium", 7f },
        { "High", 10f }
    };

    void Start()
    {
        energySystem = GetComponent<EnergySystem>();
    }

    // Determine the current energy stage
    private string GetCurrentEnergyStage()
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

    // Get current speed based on energy stage
    public float GetCurrentSpeed()
    {
        string stage = GetCurrentEnergyStage();
        return speedByStage[stage];
    }

    // Get current jump height based on energy stage
    public float GetCurrentJumpHeight()
    {
        string stage = GetCurrentEnergyStage();
        return jumpHeightByStage[stage];
    }
}
