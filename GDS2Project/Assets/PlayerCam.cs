using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public float multiplier;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;

    [Header("Fov")]
    public bool useFluentFov;
    public PlayerMovementDashing pm;
    public Rigidbody rb;
    public Camera cam;
    public float minMovementSpeed;
    public float maxMovementSpeed;
    public float minFov;
    public float maxFov;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

        yRotation += mouseX * multiplier;

        xRotation -= mouseY * multiplier;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        if (useFluentFov) HandleFov();
    }

    private void HandleFov()
    {
        float moveSpeedDif = maxMovementSpeed - minMovementSpeed;
        float fovDif = maxFov - minFov;

        float rbFlatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        float currMoveSpeedOvershoot = rbFlatVel - minMovementSpeed;
        float currMoveSpeedProgress = currMoveSpeedOvershoot / moveSpeedDif;

        float targetFov = (currMoveSpeedProgress * fovDif) + minFov;
        float currFov = cam.fieldOfView;

        // Use Lerp to smoothly change the FOV
        cam.fieldOfView = Mathf.Lerp(currFov, targetFov, Time.deltaTime * 5f);  // Adjust the speed factor if needed
    }

    public void DoFov(float endValue)
    {
        // Smoothly interpolate the FOV to the target value over time
        StartCoroutine(LerpFov(endValue, 0.25f));  // 0.25f is the duration, adjust it as needed
    }

    private IEnumerator LerpFov(float targetFov, float duration)
    {
        float startFov = cam.fieldOfView;
        float time = 0;

        while (time < duration)
        {
            cam.fieldOfView = Mathf.Lerp(startFov, targetFov, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        cam.fieldOfView = targetFov; // Ensure the target FOV is set at the end
    }

    public void DoTilt(float zTilt)
    {
        // Smoothly tilt the camera using a coroutine
        StartCoroutine(LerpTilt(zTilt, 0.25f));  // 0.25f is the duration, adjust it as needed
    }

    private IEnumerator LerpTilt(float targetTilt, float duration)
    {
        float startTilt = transform.localRotation.z;
        float time = 0;

        while (time < duration)
        {
            float zRotation = Mathf.Lerp(startTilt, targetTilt, time / duration);
            transform.localRotation = Quaternion.Euler(0, 0, zRotation);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, 0, targetTilt);  // Ensure the target tilt is set at the end
    }
}
