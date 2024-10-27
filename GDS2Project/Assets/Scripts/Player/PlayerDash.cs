using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Script to handle player dashing mechanics.
public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;

    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Dashing")]
    public float dashForce;

    public float dashUpwardForce;
    public float maxDashYSpeed;
    public float dashDuration;
    public float dashForceMultiplier = 0.5f;
    public Vector3 forceToApply;

    [Header("CameraEffects")]
    public PlayerCam cam;

    public float dashFov;

    [Header("Settings")]
    public bool useCameraForward = true;

    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCd;

    public float dashCdTimer;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;

    private Vector3 speedBeforeDash;
    public float dashGroundDrag = 2f;
    private float pmGroundDrag;
    public LayerMask dashMask;
    public float camAngle;
    public AudioSource audioSource;
    public AudioClip dashSound;

    [FormerlySerializedAs("dashIcon")]
    [FormerlySerializedAs("skillIcon")]
    public Image dashIconCD;


    private void Start()
    {
        cam = GameObject.Find("Camera").GetComponent<PlayerCam>();

        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        dashSound = Resources.Load<AudioClip>("Sound/NEW SOUNDS/NEW DASH");
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("TUT"))
            dashIconCD = GameObject.Find("dashCD").GetComponent<Image>();
        //find all the obejcts under DashIcon and get the image component
        dashIconCD.fillAmount = 0;
    }

    private void Update()
    {
        //Debug.Log("Dash cd" + dashCdTimer);

        if (Input.GetKeyDown(dashKey) && dashCdTimer <= 0)
        {
            speedBeforeDash = rb.velocity;
            Dash();
        }

        if (dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
            dashIconCD.fillAmount = dashCdTimer / dashCd;
        }
        else
        {
            dashIconCD.fillAmount = 0;
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction;
        if (useCameraForward)
        {
            // 获取摄像机前方向量的水平分量
            direction = new Vector3(playerCam.forward.x, 0, playerCam.forward.z).normalized;
        }
        else
        {
            // 获取角色前方向量的水平分量
            direction = new Vector3(orientation.forward.x, 0, orientation.forward.z).normalized;
        }

        return direction;
    }

    private void Dash()
    {
        if (dashCdTimer > 0) return;

        float camAngle = Vector3.Angle(Vector3.up, playerCam.forward) - 90;
        camAngle = Mathf.Abs(camAngle); // 确保角度为正值

        Vector3 direction = Vector3.zero;
        // if (camAngle >= 75)
        // {
        //     return;
        // }

        if (camAngle < 45)
        {
            // 摄像机倾斜角度小于45度，使用水平方向
            direction = new Vector3(playerCam.forward.x, 0, playerCam.forward.z).normalized;
            forceToApply = direction * dashForce;
        }
        else
        {
            // 摄像机倾斜角度大于或等于45度，使用原始摄像机方向
            direction = playerCam.forward.normalized;
            forceToApply = direction * dashForce * dashForceMultiplier;
            // return;
        }

        dashCdTimer = dashCd;
        dashIconCD.fillAmount = 1;
        audioSource.PlayOneShot(dashSound);

        pm.dashing = true;
        // pm.maxYSpeed = maxDashYSpeed;

        // cam.DoFovDash(dashFov, dashDuration);

        // Transform forwardT;
        //
        // if (useCameraForward)
        //     forwardT = playerCam; /// where you're looking
        // else
        //     forwardT = orientation; /// where you're facing (no up or down)


        // Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;


        if (disableGravity)
            rb.useGravity = false;
        pmGroundDrag = pm.groundDrag;
        pm.groundDrag = dashGroundDrag;
        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedDashForce()
    {
        if (resetVel)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;
        pm.maxYSpeed = 0;
        rb.velocity = speedBeforeDash;
        pm.groundDrag = pmGroundDrag;

        // cam.DoFov(pm.camFov);

        if (disableGravity)
            rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direction = forwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }
}