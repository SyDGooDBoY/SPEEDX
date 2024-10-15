using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraShake : MonoBehaviour
{
    public ObjectShake objectShake;

    public void shakeCamera()
    {
        objectShake.enabled = true;
    }
}
