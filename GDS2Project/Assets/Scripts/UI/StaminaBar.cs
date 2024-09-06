using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public RectTransform redBar;
    public RectTransform yellowBar;
    public RectTransform greenBar;

    public EnergySystem energySystem;

    private float totalWidth;

    void Start()
    {
        totalWidth = redBar.parent.GetComponent<RectTransform>().rect.width;
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

        float redWidth = Mathf.Clamp01(energyPercentage / 0.25f) * totalWidth * 0.25f;
        redBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, redWidth);

        float yellowWidth = 0;
        if (energyPercentage > 0.25f)
        {
            yellowWidth = Mathf.Clamp01((energyPercentage - 0.25f) / 0.50f) * totalWidth * 0.50f; // Adjusted range
            yellowBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, yellowWidth);
        }

        float greenWidth = 0;
        if (energyPercentage > 0.50f)
        {
            greenWidth = Mathf.Clamp01((energyPercentage - 0.50f) / 0.50f) * totalWidth * 0.50f;
            greenBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, greenWidth);
        }
    }
}