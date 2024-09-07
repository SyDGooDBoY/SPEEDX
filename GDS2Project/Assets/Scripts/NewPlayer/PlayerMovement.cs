using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动速度")]
    private float moveSpeed = 10f; //速度(暂时保留）

    public float walkSpeed = 10f; //行走速度
    public float runSpeed = 20f; //奔跑速度
    public float wallRunSpeed = 10f; //墙上移动速度
    public float climbSpeed = 5f; //爬墙速度

    [Header("地面摩擦力")]
    public float groundDrag = 5f; //地面摩擦力

    [Header("跳跃属性")]
    public float jumpForce = 12f; //跳跃力

    public float downForce = 12f; //下落力

    public float jumpCooldown = 0.25f; //跳跃cd

    public float airDrag = 0.3f; //空气阻力

    bool readyToJump;

    [Header("下蹲")]
    [Tooltip("下蹲后移动速度")]
    public float crouchMoveSpeed = 5f; //下蹲后移动速度

    public float crouchSpeed = 5f; //下蹲速度
    public float crouchYscale = 0.5f; //下蹲高度
    private float startYscale; //初始高度


    [Header("输入按键")]
    public KeyCode jumpKey = KeyCode.Space;

    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;


    [Header("地面检测(别动)")]
    //别动
    public bool isGrounded;

    public float playerHeight; //玩家高度

    public LayerMask groundMask; //地面层

    [Header("斜坡移动")]
    public float maxSlopeAngle = 40f; //最大斜坡角度

    private RaycastHit slopeHit; //斜坡检测
    private bool exitingSlope; //退出斜坡

    [Header("参考对象")]
    [Tooltip("玩家里面有个orientation对象，用来识别移动方向")]
    public Transform orientation; //玩家里面有个orientation对象，用来识别移动方向

    public PlayerClimb pc;

    //杂七杂八的变量
    float horizontalMovement;
    float verticalMovement;
    Vector3 moveDirection;
    Rigidbody rb;

    [Header("Camera Effects")]
    public PlayerCam cam;

    public float grappleFOV = 120f;
    private float camFov;

    [Header("玩家运动状态")]
    public MoveState state;

    public bool wallRunning;
    public bool climbing;
    public bool freeze;
    public bool unlimited;
    public bool restricted;

    public bool activeGrapple;

    //玩家运动状态
    public enum MoveState
    {
        freeze,
        unlimited,
        Walking,
        Running,
        wallRunning,
        climbing,
        crouching,
        Jumping
    }

    //状态处理
    private void StateHandle()
    {
        //冻结状态
        if (freeze)
        {
            state = MoveState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }

        //无限速度状态
        else if (unlimited)
        {
            state = MoveState.unlimited;
            moveSpeed = 999f;
            return;
        }
        //爬墙状态
        else if (climbing)
        {
            state = MoveState.climbing;
            moveSpeed = climbSpeed;
        }

        //墙壁跑步状态
        else if (wallRunning)
        {
            state = MoveState.wallRunning;
            moveSpeed = wallRunSpeed;
        }
        //下蹲状态
        else if (Input.GetKey(crouchKey))
        {
            state = MoveState.crouching;
            moveSpeed = crouchMoveSpeed;
        }

        //冲刺状态
        else if (isGrounded && Input.GetKey(runKey))
        {
            state = MoveState.Running;
            moveSpeed = runSpeed;
        }
        //行走状态
        else if (isGrounded)
        {
            state = MoveState.Walking;
            moveSpeed = walkSpeed;
        }
        //跳跃状态
        else
        {
            state = MoveState.Jumping;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYscale = transform.localScale.y;
        camFov = cam.GetComponent<Camera>().fieldOfView;
        cam.DoFov(camFov);
    }

    // Update is called once per frame
    void Update()
    {
        //检查地面
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        PlayerInput();
        SpeedControl();
        StateHandle();
        //增加摩擦力
        if (isGrounded && !activeGrapple)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        PlayerMove();
        ApplyDownForce();
    }

    //玩家输入
    private void PlayerInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        //跳跃
        if (Input.GetKeyDown(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); //调用跳跃cd
        }

        //下蹲
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYscale, transform.localScale.z);
            rb.AddForce(Vector3.down * crouchSpeed, ForceMode.Impulse);
        }
        //站起
        else if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
        }
    }


    //玩家移动
    private void PlayerMove()
    {
        //如果正在使用钩爪，不要移动
        if (activeGrapple)
        {
            return;
        }

        //如果冻结，不要移动
        if (restricted)
        {
            return;
        }

        //如果正在爬墙，不要移动
        if (pc.exitingWall)
        {
            return;
        }

        //计算移动方向
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
        //斜坡
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        //地面
        else if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        //空中
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airDrag, ForceMode.Force);
        }

        if (!wallRunning)
        {
            rb.useGravity = !OnSlope(); //在斜坡的时候关闭重力
        }
    }

    //控制玩家在不同情况下的速度
    private void SpeedControl()
    {
        //如果正在使用钩爪，不要限制速度
        if (activeGrapple)
        {
            return;
        }

        //斜坡上的移动速度
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        //地面和空中的移动速度
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //空中速度
            //限制移动速度
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    //跳跃
    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); //执行一次一个向上的力
    }

    //增加下落力
    private void ApplyDownForce()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * downForce, ForceMode.Force);
        }
    }

    //重置跳跃
    private void ResetJump()
    {
        exitingSlope = false;
        readyToJump = true;
    }

    //检测玩家是否在斜坡上
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            Debug.Log("斜坡角度：" + slopeAngle);
            return slopeAngle < maxSlopeAngle && slopeAngle != 0;
        }

        return false;
    }

    //获取斜坡移动方向
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    //计算钩锁跳跃速度
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
                                               + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private Vector3 velocityToSet;


    //恢复移动
    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            GetComponent<PlayerGrappling>().StopGrapple();
        }
    }

    private bool enableMovementOnNextTouch;

    //设置钩锁速度
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
        cam.DoFov(grappleFOV);
    }

    //重置钩锁
    public void ResetRestrictions()
    {
        activeGrapple = false;
        cam.DoFov(camFov);
    }


    //钩锁跳到指定位置
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3.5f);
    }
}