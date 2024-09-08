using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrappling : MonoBehaviour
{
    [Header("参考对象")]
    private PlayerMovement pm;

    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("钩锁参数")]
    public float maxGrappleDistance;

    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("钩锁CD")]
    public float grapplingCd;

    private float grapplingCdTimer;

    [Header("玩家输入")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    public bool grappling;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    // private void LateUpdate()
    // {
    //     if (grappling)
    //         lr.SetPosition(0, gunTip.position);
    // }
    //开始钩锁
    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;

        pm.freeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        // lr.enabled = true;
        // lr.SetPosition(1, grapplePoint);
    }
    //执行钩锁移动
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
    //停止钩锁
    public void StopGrapple()
    {
        pm.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

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