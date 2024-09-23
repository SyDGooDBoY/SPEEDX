using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerWall : MonoBehaviour
{
    [Header("墙壁检测")]
    public float wallDistance = 0.5f; //墙壁距离

    public float minimumJumpHeight = 1.5f; //最小跳跃高度

    public LayerMask wallMask; //墙壁层
    public LayerMask groundMask; //地面层
    private RaycastHit leftWallHit; //左墙检测
    private RaycastHit rightWallHit; //右墙检测
    private bool isWallRight; //右墙
    private bool isWallLeft; //左墙

    [Header("退出墙壁")]
    private bool exitingWall; //退出墙壁

    public float exitWallTime = 0.2f; //退出墙壁时间
    private float exitWallTimer; //退出墙壁计时器

    [Header("重力")]
    public bool useGravity;

    [Tooltip("重力反作用力")]
    public float gravityCounterForce; //重力反作用力


    [Header("玩家在墙上的移动属性")]
    public float wallRunSpeed = 10f; //墙上移动速度

    public float wallClimbSpeed = 5f; //墙上爬行速度
    public float WallJumpUpForce = 10f; //墙上跳跃力
    public float WallJumpSideForce = 70f; //墙上跳跃侧向力
    public float wallRunMaxTime = 2f; //墙上最大时间
    private float wallRunTimer; //墙上时间

    [Header("玩家输入")]
    public KeyCode jumpKey = KeyCode.Space; //跳跃

    public KeyCode upwardsRunKey = KeyCode.LeftShift; //墙上向上爬
    public KeyCode downwardRunKey = KeyCode.LeftControl; //墙上向下爬
    private bool upwardsRunning; //向上爬
    private bool downwardRunning; //向下爬

    private float horizontalInput; //水平输入

    private float verticalInput; //垂直输入

    [Header("参考对象")]
    [Tooltip("玩家里面有个orientation对象，用来识别移动方向")]
    public Transform orientation; //玩家里面有个orientation对象，用来识别移动方向

    public PlayerCam cam; //摄像机

    public float onWallFOV = 120f; //墙上视野

    // private PlayerGrab pg;
    private float camFov; //摄像机视野
    private PlayerMovement playerMovement; //玩家移动脚本
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // pg = GetComponent<PlayerGrab>();
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        cam = GameObject.Find("Camera").GetComponent<PlayerCam>();

        camFov = cam.GetComponent<Camera>().fieldOfView;
        cam.DoFov(camFov);
    }

    // Update is called once per frame
    void Update()
    {
        WallCheck();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (playerMovement.wallRunning)
        {
            WallRunMovement();
        }
    }

    //检测墙壁
    private void WallCheck()
    {
        isWallLeft =
            Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance, wallMask); //左墙
        isWallRight =
            Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance, wallMask); //右墙
    }

    //检测是否在地面上方
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight, groundMask);
    }

    //墙壁交互的状态
    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardRunning = Input.GetKey(downwardRunKey);
        //墙上跑
        if ((isWallLeft || isWallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            //在墙上跑
            if (!playerMovement.wallRunning)
            {
                StartWallRun();
            }

            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if (wallRunTimer <= 0 && playerMovement.wallRunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            //墙上跳跃
            if (Input.GetKeyDown(jumpKey))
            {
                WallJump();
            }
        }

        //离开墙壁
        else if (exitingWall)
        {
            if (playerMovement.wallRunning)
            {
                StopWallRun();
            }

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }

        else
        {
            if (playerMovement.wallRunning)
            {
                StopWallRun();
            }
        }
    }

    //开始墙上跑
    private void StartWallRun()
    {
        playerMovement.wallRunning = true;
        wallRunTimer = wallRunMaxTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //应用摄像机效果
        cam.DoFov(onWallFOV);
        if (isWallLeft)
        {
            cam.DoTilt(-5f);
        }

        if (isWallRight)
        {
            cam.DoTilt(5f);
        }
    }

    //墙上移动
    private void WallRunMovement()
    {
        rb.useGravity = useGravity;

        // rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //墙上向前跑
        rb.AddForce(wallForward * wallRunSpeed, ForceMode.Force);

        //墙上向上或者向下跑
        if (upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }

        if (downwardRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }

        //对墙的力
        if (!(isWallLeft && horizontalInput > 0) && !(isWallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        rb.useGravity = true;
        playerMovement.wallRunning = false;
        cam.DoFov(camFov);
        cam.DoTilt(0f);
    }

    private void WallJump()
    {
        // if (pg.holding || pg.exitingLedge) return;
        exitingWall = true;
        exitWallTimer = exitWallTime;
        Vector3 wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * WallJumpUpForce + wallNormal * WallJumpSideForce;
        //重置 y 轴速度并添加跳跃力
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        playerMovement.canControlInAir = true;
    }
}