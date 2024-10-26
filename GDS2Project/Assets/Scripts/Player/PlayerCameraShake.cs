using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraShake : MonoBehaviour
{
    public ObjectShake objectShake;

    [Header("Time Slowing")]
    public float slowMotionFactor = 0.5f;
    public float slowDuration = 0.2f;

    public void shakeCamera()
    {
        objectShake.enabled = true;

        SlowMotion(true);
        StartCoroutine(slowTime());
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
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    IEnumerator slowTime()
    {
        yield return new WaitForSeconds(slowDuration * slowMotionFactor);
        SlowMotion(false);
    }
}
