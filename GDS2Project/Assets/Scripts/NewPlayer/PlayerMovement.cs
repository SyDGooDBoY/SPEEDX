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


    [Header("地面检测")]
    //别动
    bool isGrounded;

    public float playerHeight; //玩家高度

    public LayerMask groundMask; //地面层

    [Header("斜坡移动")]
    public float maxSlopeAngle = 40f; //最大斜坡角度

    private RaycastHit slopeHit; //斜坡检测
    private bool exitingSlope; //退出斜坡

    [Header("识别移动方向的对象")]
    [Tooltip("玩家里面有个orientation对象，用来识别移动方向")]
    public Transform orientation; //玩家里面有个orientation对象，用来识别移动方向

    //杂七杂八的变量
    float horizontalMovement;
    float verticalMovement;
    Vector3 moveDirection;
    Rigidbody rb;

    [Header("玩家运动状态")]
    public MoveState state;

    //玩家运动状态
    public enum MoveState
    {
        Walking,
        Running,
        crouching,
        Jumping
    }

    //状态处理
    private void StateHandle()
    {
        //下蹲状态
        if (Input.GetKey(crouchKey))
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
        if (isGrounded)
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

        rb.useGravity = !OnSlope(); //在斜坡的时候关闭重力
    }

    //控制玩家在不同情况下的速度
    private void SpeedControl()
    {
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
}