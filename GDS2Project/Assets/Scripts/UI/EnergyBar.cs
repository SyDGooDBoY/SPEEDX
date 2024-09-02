using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    // 3 Segment of Energy Bar
    public Image leftSegment;  
    public Image middleSegment; 
    public Image rightSegment;

    private AbilityManager abilityManager;

    void Start()
    {
        abilityManager = GetComponent<AbilityManager>();
        UpdateEnergyBar(); // Initial Update
    }

    public void SetEnergy(float energy)
    {
        abilityManager.energySystem.SetEnergy(energy); 
        UpdateEnergyBar(); 
    }

    // Update display
    private void UpdateEnergyBar()
    {
        float currentEnergy = abilityManager.energySystem.GetCurrentEnergy();

        // Calculate the fill ratio of the left, middle and right segments
        float leftFill = Mathf.Clamp01(currentEnergy / AbilityManager.LOW_THRESHOLD); 
        float middleFill = Mathf.Clamp01((currentEnergy - AbilityManager.LOW_THRESHOLD) 
            / (AbilityManager.MEDIUM_THRESHOLD - AbilityManager.LOW_THRESHOLD)); 
        float rightFill = Mathf.Clamp01((currentEnergy - AbilityManager.MEDIUM_THRESHOLD) 
            / (AbilityManager.HIGH_THRESHOLD - AbilityManager.MEDIUM_THRESHOLD));

        // Update the fill amount
        leftSegment.fillAmount = leftFill;
        middleSegment.fillAmount = middleFill;
        rightSegment.fillAmount = rightFill;
    }
}
