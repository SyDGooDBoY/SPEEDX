using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    public Transform orientationPlayer; //跟踪对象面对的方向
    public Transform camHolder; //摄像机的父对象

    [Header("XY灵敏度")]
    public float sensX;

    public float sensY;

    [Header("旋转角度")]
    float rotationY;

    float rotationX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90, 90);

        camHolder.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientationPlayer.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}