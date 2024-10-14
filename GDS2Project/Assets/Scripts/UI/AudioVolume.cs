using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolume : MonoBehaviour
{
    public Image audioImage;

    private void Start()
    {
        //change audio Image to the other one
        if (AudioListener.pause)
        {
            audioImage.sprite = Resources.Load<Sprite>("Mute");
        }
        else
        {
            audioImage.sprite = Resources.Load<Sprite>("Sound");
        }
    }

    public void audioControl()
    {
        if (AudioListener.pause)
        {
            AudioListener.pause = false;
            audioImage.sprite = Resources.Load<Sprite>("Sound");
        }
        else
        {
            DisableAudio();
            audioImage.sprite = Resources.Load<Sprite>("Mute");
        }
    }

    public void DisableAudio()
    {
        AudioListener.pause = true;
    }
}