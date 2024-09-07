using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [Header("�ο�����")]
    [Tooltip("��������и�orientation��������ʶ���ƶ�����")]
    public Transform orientation; //��������и�orientation��������ʶ���ƶ�����

    public Rigidbody rb;
    public Transform cam;
    public PlayerMovement pm;

    [Header("����")]
    public float moveToLedgeSpeed;

    public float maxLedgeGrabDistance;

    public float minTimeOnLedge;
    private float timeOnLedge;

    public bool holding;

    [Header("������Ծ")]
    public KeyCode jumpKey = KeyCode.Space;

    public float ledgeJumpForwardForce;
    public float ledgeJumpUpwardForce;

    [Header("���Ҽ��")]
    public float ledgeDetectionLength;

    public float ledgeSphereCastRadius;
    public LayerMask whatIsLedge;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;

    [Header("�˳�����")]
    public bool exitingLedge;

    public float exitLedgeTime;
    private float exitLedgeTimer;

    private void Update()
    {
        LedgeDetection();
        SubStateMachine();
    }

    private void SubStateMachine()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool anyInputKeyPressed = horizontalInput != 0 || verticalInput != 0;

        //ץס��Ե
        if (holding)
        {
            FreezeRigidbodyOnLedge();

            timeOnLedge += Time.deltaTime;

            if (timeOnLedge > minTimeOnLedge && anyInputKeyPressed) ExitLedgeHold();

            if (Input.GetKeyDown(jumpKey)) LedgeJump();
        }

        //�˳���Ե
        else if (exitingLedge)
        {
            if (exitLedgeTimer > 0) exitLedgeTimer -= Time.deltaTime;
            else exitingLedge = false;
        }
    }

    //����Ե
    private void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit,
            ledgeDetectionLength, whatIsLedge);

        if (!ledgeDetected) return;

        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

        if (ledgeHit.transform == lastLedge) return;

        if (distanceToLedge < maxLedgeGrabDistance && !holding) EnterLedgeHold();
    }

    //��Ծ
    private void LedgeJump()
    {
        ExitLedgeHold();

        Invoke(nameof(DelayedJumpForce), 0.05f);
    }

    //�ӳ���Ծ
    private void DelayedJumpForce()
    {
        Vector3 forceToAdd = cam.forward * ledgeJumpForwardForce + orientation.up * ledgeJumpUpwardForce;
        rb.velocity = Vector3.zero;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    //�����Ե
    private void EnterLedgeHold()
    {
        holding = true;

        pm.unlimited = true;
        pm.restricted = true;

        currLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    //�������
    private void FreezeRigidbodyOnLedge()
    {
        rb.useGravity = false;

        Vector3 directionToLedge = currLedge.position - transform.position;
        float distanceToLedge = Vector3.Distance(transform.position, currLedge.position);

        //�ƶ�����Ե
        if (distanceToLedge > 1f)
        {
            if (rb.velocity.magnitude < moveToLedgeSpeed)
                rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime);
        }

        //ץס��Ե
        else
        {
            if (!pm.freeze) pm.freeze = true;
            if (pm.unlimited) pm.unlimited = false;
        }

        //����������⣬�˳�
        if (distanceToLedge > maxLedgeGrabDistance) ExitLedgeHold();
    }

    //�˳���Ե
    private void ExitLedgeHold()
    {
        exitingLedge = true;
        exitLedgeTimer = exitLedgeTime;

        holding = false;
        timeOnLedge = 0f;

        pm.restricted = false;
        pm.freeze = false;

        rb.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(ResetLastLedge), 1f);
    }

    //�������һ����Ե
    private void ResetLastLedge()
    {
        lastLedge = null;
    }
}