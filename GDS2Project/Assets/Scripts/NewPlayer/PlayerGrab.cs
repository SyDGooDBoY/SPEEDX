using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [Header("参考对象")]
    [Tooltip("玩家里面有个orientation对象，用来识别移动方向")]
    public Transform orientation; //玩家里面有个orientation对象，用来识别移动方向

    public Rigidbody rb;
    public Transform cam;
    public PlayerMovement pm;

    [Header("攀岩")]
    public float moveToLedgeSpeed;

    public float maxLedgeGrabDistance;

    public float minTimeOnLedge;
    private float timeOnLedge;

    public bool holding;

    [Header("攀岩跳跃")]
    public KeyCode jumpKey = KeyCode.Space;

    public float ledgeJumpForwardForce;
    public float ledgeJumpUpwardForce;

    [Header("攀岩检测")]
    public float ledgeDetectionLength;

    public float ledgeSphereCastRadius;
    public LayerMask whatIsLedge;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;

    [Header("退出攀岩")]
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

        //抓住边缘
        if (holding)
        {
            FreezeRigidbodyOnLedge();

            timeOnLedge += Time.deltaTime;

            if (timeOnLedge > minTimeOnLedge && anyInputKeyPressed) ExitLedgeHold();

            if (Input.GetKeyDown(jumpKey)) LedgeJump();
        }

        //退出边缘
        else if (exitingLedge)
        {
            if (exitLedgeTimer > 0) exitLedgeTimer -= Time.deltaTime;
            else exitingLedge = false;
        }
    }

    //检测边缘
    private void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit,
            ledgeDetectionLength, whatIsLedge);

        if (!ledgeDetected) return;

        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

        if (ledgeHit.transform == lastLedge) return;

        if (distanceToLedge < maxLedgeGrabDistance && !holding) EnterLedgeHold();
    }

    //跳跃
    private void LedgeJump()
    {
        ExitLedgeHold();

        Invoke(nameof(DelayedJumpForce), 0.05f);
    }

    //延迟跳跃
    private void DelayedJumpForce()
    {
        Vector3 forceToAdd = cam.forward * ledgeJumpForwardForce + orientation.up * ledgeJumpUpwardForce;
        rb.velocity = Vector3.zero;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    //进入边缘
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

    //冻结刚体
    private void FreezeRigidbodyOnLedge()
    {
        rb.useGravity = false;

        Vector3 directionToLedge = currLedge.position - transform.position;
        float distanceToLedge = Vector3.Distance(transform.position, currLedge.position);

        //移动到边缘
        if (distanceToLedge > 1f)
        {
            if (rb.velocity.magnitude < moveToLedgeSpeed)
                rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime);
        }

        //抓住边缘
        else
        {
            if (!pm.freeze) pm.freeze = true;
            if (pm.unlimited) pm.unlimited = false;
        }

        //如果出现问题，退出
        if (distanceToLedge > maxLedgeGrabDistance) ExitLedgeHold();
    }

    //退出边缘
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

    //重置最后一个边缘
    private void ResetLastLedge()
    {
        lastLedge = null;
    }
}