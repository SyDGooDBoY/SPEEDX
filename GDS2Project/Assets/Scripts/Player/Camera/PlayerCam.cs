using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    public Transform orientationPlayer; // 跟踪对象面对的方向
    public Transform camHolder; // 摄像机的父对象

    [Header("旋转速度")]
    public float rotationSpeed = 1.0f; // 控制摄像机旋转的速度

    float rotationY;
    float rotationX;

    private PlayerMovement pm;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!pm.inputEnabled) return;
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90, 90);

        // 使用插值平滑旋转
        Quaternion targetRotationCam = Quaternion.Euler(rotationX, rotationY, 0);
        Quaternion targetRotationOrientation = Quaternion.Euler(0, rotationY, 0);

        camHolder.localRotation =
            Quaternion.Lerp(camHolder.localRotation, targetRotationCam, Time.deltaTime * 30); // 可调整插值速度
        orientationPlayer.rotation =
            Quaternion.Slerp(orientationPlayer.rotation, targetRotationOrientation,
                Time.deltaTime * 30); // 使用Slerp以保持更自然的旋转
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f).SetEase(Ease.InOutQuad); // 添加缓动函数以平滑过渡
    }

    public void DoFovDash(float endValue, float duration)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, duration).SetEase(Ease.InOutQuad); // 添加缓动函数以平滑过渡
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f).SetEase(Ease.InOutQuad); // 同样使用缓动
    }
}