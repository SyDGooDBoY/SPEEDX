using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainTools;

public class PauseBGM : MonoBehaviour
{
    private PauseMenu pauseMenu;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
        pauseMenu = GameObject.Find("UIManager").GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        StopBGM();
    }

    private void StopBGM()
    {
        if (pauseMenu.GameIsPaused == true)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
    }
}