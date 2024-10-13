using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public RectTransform fill;


    public EnergySystem energySystem;

    private float totalWidth;

    void Start()
    {
        totalWidth = fill.GetComponent<RectTransform>().rect.width;
        UpdateEnergyBar();
    }

    void Update()
    {
        UpdateEnergyBar();
    }

    void UpdateEnergyBar()
    {
        float currentEnergy = energySystem.GetCurrentEnergy();
        float maxEnergy = AbilityManager.HIGH_THRESHOLD;


        float energyPercentage = currentEnergy / maxEnergy;

        float fillWidth = energyPercentage * totalWidth;


        fill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fillWidth);
    }
}