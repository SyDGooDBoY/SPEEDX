using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDetectionSystem : MonoBehaviour
{
    public Transform nextTeleportPoint; // next teleport point's Transform after the event succeeds
    public GameObject celebrationUI;
    public GameObject promptUI; 

    [Tooltip("Require player to press")]
    public KeyCode requiredKey = KeyCode.E;  // The button that the player needs to press
    public float actionTimeWindow = 3f;  // Time window for key detection
    public float slowMotionFactor = 0.2f; //Time slowdown multiplier

    private bool isInArea = false;    // Is the player in the area?
    private bool actionExecuted = false; // Whether the specific action is executed
    private float originalTimeScale;

    public IEventCondition eventCondition; // Event condition interface

    public enum EventType { Jump, GrapplingHook }
    public EventType eventType;  // enumeration variable, select it in the Inspector


    void Start()
    {
        originalTimeScale = Time.timeScale; // Save the initial time scale
        if (celebrationUI != null) 
        { 
            celebrationUI.SetActive(false); 
        }
        if (promptUI != null)
        {
            promptUI.SetActive(false); 
        }

        // Initialize the corresponding IEventCondition according to the selected event type
        switch (eventType)
        {
            case EventType.Jump:
                eventCondition = new JumpEventCondition();
                break;
            case EventType.GrapplingHook:
                eventCondition = new GrapplingHookEventCondition();
                break;
            default:
                eventCondition = new DefaultEventCondition(); 
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInArea = true;
            actionExecuted = false;
            ShowPromptUI();
            SlowMotion(true);  // Time slowing
            StartCoroutine(WaitForAction(other.transform)); // Start detecting key input
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInArea = false;
            HidePromptUI();
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
                    HidePromptUI();
                    ShowCelebrationUI();
                    SlowMotion(false);
                    yield break;
                }
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
        return Input.GetKeyDown(requiredKey); 
    }

    // Display the prompt UI
    void ShowPromptUI()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(true); 
        }
    }

    // Hide the prompt UI
    void HidePromptUI()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
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
