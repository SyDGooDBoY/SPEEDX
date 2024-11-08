using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergySystem : MonoBehaviour
{
    [Header("Energy VFX")]
    public GameObject energyBar;

    private Image[] energyBarImages; // to store images of energy bar
    public GameObject fullscreenEffect; // boost fullscreen effect
    public Image boostButton;

    [Header("Energy Recover&Decrease")]
    public float energyRecoveryRate = 20f; // Energy recovery rate per second

    public float energyDecreaseRate = 80f; // Energy decrease rate when stopped
    private float maxEnergy = AbilityManager.HIGH_THRESHOLD; // Maximum energy 
    private float currentEnergy; // Current energy for Get function

    private bool isRecovering = false; // Whether energy is currently recovering
    private float recoveryDelay = 1f; // Delay before starting to recover energy
    private float recoveryTimer = 0f; // Timer to track when to start recovery

    public float stopMoveDelay = 1f; // energy decreasing delay after stopping

    [HideInInspector]
    public float stopMoveTimer = 0f; // Timer to track stop movement delay

    [Header("Boosting")]
    [HideInInspector]
    public bool isBoosting = false;

    public float boostEnergyConsumptionRate = 40f;

    [Header("Boosting Camera")]
    public PlayerCam cam;

    public float boostFOV = 120f;
    private float camFov;

    private Image staminaBar;
    private Color staminaColor;
    private Material skybox;
    private Color skyboxColor;

    void Start()
    {
        // Initialize start energy to stage 1
        cam = GameObject.Find("Camera").GetComponent<PlayerCam>(); // Find the camera object in the scene

        currentEnergy = AbilityManager.LOW_THRESHOLD;
        Debug.Log("Current Energy: " + currentEnergy);

        if (fullscreenEffect != null)
        {
            fullscreenEffect.SetActive(false);
        }

        // get all Image components in energy bar
        if (energyBar != null)
        {
            energyBarImages = energyBar.GetComponentsInChildren<Image>();
        }

        UpdateEnergyBarTransparency();

        // get initial cam FOV
        camFov = cam.GetComponent<Camera>().fieldOfView;
        staminaBar = GameObject.Find("StaminaBar/Fill").GetComponent<Image>();
        staminaColor = staminaBar.color;
        //find skybox called skybox in the lighting settings
        skybox = RenderSettings.skybox;
        skybox.SetColor("_Tint", Color.white);

        skyboxColor = skybox.GetColor("_Tint");
    }

    void Update()
    {
        if (isBoosting)
        {
            ConsumeEnergyDuringBoost(); // boost consumption
        }
        else
        {
            if (isRecovering)
            {
                RecoverEnergy();
            }
            else
            {
                if (currentEnergy >= maxEnergy)
                {
                    // If energy is full, start counting the stop timer before decreasing energy
                    stopMoveTimer += Time.deltaTime;
                    if (stopMoveTimer >= stopMoveDelay)
                    {
                        DecreaseEnergyOverTime(); // Start decreasing energy after delay
                    }
                }
                else
                {
                    DecreaseEnergyOverTime(); // Normal energy decrease
                }
            }
        }

        // Boost Logic
        if (Input.GetKeyDown(KeyCode.F))
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

        // UpdateEnergyBarTransparency();
        UpdateBoostButton();
    }

    // Updated energy bar transparency
    private void UpdateEnergyBarTransparency()
    {
        if (energyBarImages != null)
        {
            // 0-400 energy --> 0.2f-1.0f transparency
            float alpha = Mathf.Lerp(0.2f, 1.0f, currentEnergy / maxEnergy);

            // Iterate over all Image components
            foreach (var image in energyBarImages)
            {
                Color currentColor = image.color;
                currentColor.a = alpha;
                image.color = currentColor; // update transparency
            }
        }
    }

    // Updated boost button transparency
    private void UpdateBoostButton()
    {
        if (boostButton != null)
        {
            Color imageColor = boostButton.color;

            if (isBoosting)
            {
                // If in boosy state, transparency 60%
                imageColor.a = 0.6f;
            }
            else if (currentEnergy >= maxEnergy)
            {
                // If the energy is full but not boosting, transparency 100%
                imageColor.a = 1.0f;
            }
            else
            {
                // Normally invisible
                imageColor.a = 0.0f;
            }

            // Apply the new transparency
            boostButton.color = imageColor;
        }
    }

    // enter boost state
    public void EnterBoost()
    {
        isBoosting = true;
        if (fullscreenEffect != null)
        {
            fullscreenEffect.SetActive(true); // active
        }

        staminaBar.color = new Color(0.96f, 0.0f, 0.63f, 1.0f);
        skybox.SetColor("_Tint", new Color(0.96f, 0.0f, 0.63f, 1.0f));
        // change FOV
        cam.DoFov(boostFOV);
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

        if (staminaBar != null)
        {
            staminaBar.color = staminaColor;
        }

        skybox.SetColor("_Tint", skyboxColor);
        // change cam FOV back
        cam.DoFov(camFov);
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