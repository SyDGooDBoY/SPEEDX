using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDetectionSystem : MonoBehaviour
{
    public Transform nextTeleportPoint; // next teleport point's Transform after the event succeeds
    public GameObject celebrationUI; 

    [Tooltip("Require player to press £º'E', 'Mouse0' (Left), 'Mouse1' (Right)")]
    public string requiredKey = "E";  // The button that the player needs to press
    public float actionTimeWindow = 3f;  // Time window for key detection
    public float slowMotionFactor = 0.1f; //Time slowdown multiplier

    private bool isInArea = false;    // Is the player in the area?
    private bool actionExecuted = false; // Whether the specific action is executed
    private float originalTimeScale;

    public IEventCondition eventCondition; // Event condition interface


    void Start()
    {
        originalTimeScale = Time.timeScale; // Save the initial time scale
        if (celebrationUI != null) 
        { 
            celebrationUI.SetActive(false); 
        }

        // Assign event condition
        if (eventCondition == null)
        {
            Debug.LogWarning("EventCondition not assigned, using default success condition.");
            eventCondition = new DefaultEventCondition(); 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInArea = true;
            actionExecuted = false;
            SlowMotion(true);  // Time slowing
            StartCoroutine(WaitForAction(other.transform)); // Start detecting key input
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInArea = false;
            SlowMotion(false);  // Return to normal time
            StopAllCoroutines(); // Player leaves the area, Stops detecting
        }
    }

    IEnumerator WaitForAction(Transform player)
    {
        float elapsedTime = 0f;

        while (elapsedTime < actionTimeWindow / slowMotionFactor) // Calculate according to slow time
        {
            elapsedTime += Time.deltaTime;

            // Detect keyboard and mouse input
            if (isInArea && IsCorrectInput())
            {
                // Success judgment of calling event conditions
                if (eventCondition.CheckSuccess())
                {
                    actionExecuted = true;
                    //UpdateCheckpoint(player); // Update the teleport point
                    ShowCelebrationUI();
                    SlowMotion(false);
                    yield break;
                }

                //actionExecuted = true;
                ////UpdateCheckpoint(player); // Update the teleport point
                //ShowCelebrationUI();
                //SlowMotion(false);
                //yield break;
            }

            yield return null;
        }

        // Timed out and unsuccessful execution
        if (!actionExecuted)
        {
            //TeleportPlayerToCheckpoint(player); // Teleport the player to the teleport point
            SlowMotion(false);  //Resume time scale
        }
    }

    // Time slowdown Logic
    void SlowMotion(bool enable)
    {
        if (enable)
        {
            Time.timeScale = slowMotionFactor; 
            Time.fixedDeltaTime = Time.timeScale * 0.02f; // Keep physics updates in sync
        }
        else
        {
            Time.timeScale = originalTimeScale; 
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    // Detect if a specific button is pressed
    bool IsCorrectInput()
    {
        if (requiredKey == "Mouse0" && Input.GetMouseButtonDown(0)) // Left
        {
            return true;
        }
        else if (requiredKey == "Mouse1" && Input.GetMouseButtonDown(1)) // Right
        {
            return true;
        }
        else if (Input.GetKeyDown(requiredKey)) // Key
        {
            return true;
        }

        return false;
    }

    //// 
    //void UpdateCheckpoint(Transform player)
    //{
    //    currentCheckpoint.position = player.position; 
    //}

    //// 
    //void TeleportPlayerToCheckpoint(Transform player)
    //{
    //    player.position = currentCheckpoint.position; 
    //}

    // Display the celebration UI
    void ShowCelebrationUI()
    {
        if (celebrationUI != null)
        {
            celebrationUI.SetActive(true);
            // displayed for a few seconds and then hides
            StartCoroutine(HideCelebrationUI());
        }
    }

    IEnumerator HideCelebrationUI()
    {
        yield return new WaitForSeconds(2f); 
        celebrationUI.SetActive(false); 
    }
}

public interface IEventCondition
{
    bool CheckSuccess(); // Interface for event conditions
}

// Default
public class DefaultEventCondition : IEventCondition
{
    public bool CheckSuccess()
    {
        return true;
    }
}
