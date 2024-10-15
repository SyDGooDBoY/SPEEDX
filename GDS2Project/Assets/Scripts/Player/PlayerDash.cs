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
        Debug.Log("Dash cd" + dashCdTimer);

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
        if (useCameraForward)
            return playerCam.forward.normalized; // 使用相机前方方向
        else
            return orientation.forward.normalized; // 使用角色前方方向
    }

    private void Dash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;
        dashIconCD.fillAmount = 1;
        Vector3 horizontalForward = new Vector3(playerCam.forward.x, 0, playerCam.forward.z).normalized;
        float angle = Vector3.Angle(horizontalForward, playerCam.forward);

        // 检查角度是否超过设定的限制（例如45度）
        if (angle > camAngle)
        {
            return; // 如果角度过大，则不执行冲刺
        }

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

        Vector3 direction = GetDirection();

        // Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;
        Vector3 forceToApply = direction * dashForce;

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