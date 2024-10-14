using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrappling : MonoBehaviour
{
    [Header("�ο�����")]
    private PlayerMovement pm;

    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("��������")]
    public float maxGrappleDistance;

    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("����CD")]
    public float grapplingCd;

    private float grapplingCdTimer;

    [Header("�������")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    public bool grappling;

    //public float grappleConsumption = 50f;
    private EnergySystem energySystem;

    [Header("Energy Recover")]
    public float energyRecoverAmount = 30f;

    public PlayerShootTeleport playerShootTeleport;

    [Header("Sound")]
    public AudioClip grappleSound;

    private AudioSource audioSource;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        energySystem = GetComponent<EnergySystem>();
        playerShootTeleport = GetComponent<PlayerShootTeleport>();
        cam = GameObject.Find("Camera").transform;
        gunTip = GameObject.Find("shooting point").transform;
        audioSource = GetComponent<AudioSource>();
        grappleSound = Resources.Load<AudioClip>("Sound/grapple sound");
    }

    private void Update()
    {
        if (!pm.inputEnabled || playerShootTeleport.GetCurrentShootPhase() != 0)
            return; // Prevent grappling during aiming or shooting
        if (Input.GetKeyDown(grappleKey) && IsValidGrapplePoint()) StartGrapple();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    private bool IsValidGrapplePoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            return true;
        }

        return false;
    }

    // private void LateUpdate()
    // {
    //     if (grappling)
    //         lr.SetPosition(0, gunTip.position);
    // }
    //��ʼ����
    private void StartGrapple()
    {
        GetComponent<PlayerSwinging>().StopSwing();

        if (grapplingCdTimer > 0) return;

        grappling = true;

        pm.freeze = true;

        RaycastHit hit;

        grapplingCdTimer = grapplingCd;

        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            energySystem.RecoverEnergy(energyRecoverAmount);

            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        if (grappleSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(grappleSound);
        }

        // lr.enabled = true;
        // lr.SetPosition(1, grapplePoint);
    }

    //ִ�й����ƶ�
    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1.5f);
    }

    //ֹͣ����
    public void StopGrapple()
    {
        pm.freeze = false;

        grappling = false;


        // lr.enabled = false;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}