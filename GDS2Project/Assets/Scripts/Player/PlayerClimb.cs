using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    [Header("参考对象")]
    [Tooltip("玩家里面有个orientation对象，用来识别移动方向")]
    public Transform orientation; //玩家里面有个orientation对象，用来识别移动方向

    public Rigidbody rb;
    public PlayerMovement pm;
    public PlayerGrab pg;
    public LayerMask whatIsWall;

    [Header("爬墙属性")]
    public float climbSpeed;

    public float maxClimbTime;
    private float climbTimer;

    private bool climbing;

    [Header("从墙上跳")]
    public float climbJumpUpForce;

    public float climbJumpBackForce;

    public KeyCode jumpKey = KeyCode.Space;
    public int climbJumps;
    private int climbJumpsLeft;

    [Header("检测垂直的墙")]
    public float detectionLength;

    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("离开墙")]
    public bool exitingWall;

    public float exitWallTime;
    private float exitWallTimer;


    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        WallCheck();
        StateMachine();

        if (climbing && !exitingWall) ClimbingMovement();
    }

    //墙壁交互的状态
    private void StateMachine()
    {
        //攀岩状态
        // // if (pg.holding)
        // {
        //     if (climbing) StopClimbing();
        // }

        //爬墙（只有小于最大角度，按下W键才可以爬）
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!climbing && climbTimer > 0) StartClimbing();

            //计时器
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();
        }
        //离开墙
        else if (exitingWall)
        {
            if (climbing) StopClimbing();

            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if (exitWallTimer < 0) exitingWall = false;
        }
        //没有墙
        else
        {
            if (climbing) StopClimbing();
        }

        if (wallFront && Input.GetKeyDown(jumpKey) && climbJumpsLeft > 0)
        {
            ClimbJump();
        }
    }

    //墙壁检测
    private void WallCheck()
    {
        //检测前方是否有墙
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit,
            detectionLength, whatIsWall);
        //检测前方墙的角度
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall ||
                       Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if (pm.isGrounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    //开始爬墙
    private void StartClimbing()
    {
        climbing = true;
        pm.climbing = true;
        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    //爬墙运动
    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    //结束爬墙
    private void StopClimbing()
    {
        climbing = false;
        pm.climbing = false;
    }

    private void ClimbJump()
    {
        if (pm.isGrounded) return;
        // if (pg.holding || pg.exitingLedge) return;
        exitingWall = true;
        exitWallTimer = exitWallTime;
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        climbJumpsLeft--;
        pm.canControlInAir = true;
    }
}