using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintTutorial : MonoBehaviour
{
    public GameObject moveUI;
    public GameObject boostingUI;
    public GameObject promptUI;


    public EnergySystem playerEnergySystem;

    private bool actionExecuted = false; 
    private float slowMotionFactor = 0.2f;
    private float originalTimeScale = 1f; 
    private float actionTimeWindow = 5f; 

    void Start()
    {
        if (moveUI != null) { moveUI.SetActive(false); }
        if (boostingUI != null) { boostingUI.SetActive(false); }
        if (promptUI != null) { promptUI.SetActive(false); }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerEnergySystem.GetCurrentEnergy() >= AbilityManager.HIGH_THRESHOLD && !actionExecuted)
            {
                boostingUI.SetActive(true);
                moveUI.SetActive(false);
                promptUI.SetActive(false);
                SlowMotion(true);
                StartCoroutine(WaitForAction(other.transform));
            }
            else if (playerEnergySystem.GetCurrentEnergy() < AbilityManager.HIGH_THRESHOLD 
                && !playerEnergySystem.isBoosting)
            {
                moveUI.SetActive(true);
                boostingUI.SetActive(false);
                promptUI.SetActive(false);
            }
            else if (playerEnergySystem.GetCurrentEnergy() < AbilityManager.HIGH_THRESHOLD 
                && playerEnergySystem.isBoosting)
            {
                promptUI.SetActive(true);
                moveUI.SetActive(false);
                boostingUI.SetActive(false);
            }

            else
            {
                moveUI.SetActive(false);
                boostingUI.SetActive(false);
                promptUI.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    if (moveUI != null)
        //    {
        //        moveUI.SetActive(false);
        //    }
        //}

        if (other.CompareTag("Player"))
        {
            actionExecuted = false;
            moveUI.SetActive(false);
            boostingUI.SetActive(false);
            promptUI.SetActive(false);
            SlowMotion(false);  
            StopAllCoroutines();
        }
    }

    IEnumerator WaitForAction(Transform player)
    {
        float elapsedTime = 0f;

        while (elapsedTime < actionTimeWindow / slowMotionFactor) 
        {
            elapsedTime += Time.deltaTime;

            
            if (Input.GetKeyDown(KeyCode.F)) 
            {
                actionExecuted = true;

                moveUI.SetActive(false);
                boostingUI.SetActive(false);
                SlowMotion(false); 
                yield break; 
            }

            yield return null;
        }

        if (!actionExecuted)
        {
            //Debug.Log("Action timed out. No input received.");
            SlowMotion(false); 
        }
    }

    void SlowMotion(bool enable)
    {
        if (enable)
        {
            Time.timeScale = slowMotionFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f; 
        }
        else
        {
            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }


    private void ShowCelebrationUI()
    {
        Debug.Log("Action executed successfully!");
    }
}
