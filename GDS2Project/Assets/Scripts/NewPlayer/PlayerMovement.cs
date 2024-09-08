using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Header("�ƶ��ٶ�")]
    private float moveSpeed = 10f; //�ٶ�(��ʱ������

    public float walkSpeed = 10f; //�����ٶ�
    public float runSpeed = 20f; //�����ٶ�
    public float wallRunSpeed = 10f; //ǽ���ƶ��ٶ�
    public float climbSpeed = 5f; //��ǽ�ٶ�

    [Header("����Ħ����")]
    public float groundDrag = 5f; //����Ħ����

    [Header("��Ծ����")]
    public float jumpForce = 12f; //��Ծ��

    public float downForce = 12f; //������

    public float jumpCooldown = 0.25f; //��Ծcd

    public float airDrag = 0.3f; //��������

    bool readyToJump;

    [Header("�¶�")]
    [Tooltip("�¶׺��ƶ��ٶ�")]
    public float crouchMoveSpeed = 5f; //�¶׺��ƶ��ٶ�

    public float crouchSpeed = 5f; //�¶��ٶ�
    public float crouchYscale = 0.5f; //�¶׸߶�
    private float startYscale; //��ʼ�߶�


    [Header("���밴��")]
    public KeyCode jumpKey = KeyCode.Space;

    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;


    [Header("������(��)")]
    //��
    public bool isGrounded;

    public float playerHeight; //��Ҹ߶�

    public LayerMask groundMask; //�����

    [Header("б���ƶ�")]
    public float maxSlopeAngle = 40f; //���б�½Ƕ�

    private RaycastHit slopeHit; //б�¼��
    private bool exitingSlope; //�˳�б��

    [Header("�ο�����")]
    [Tooltip("��������и�orientation��������ʶ���ƶ�����")]
    public Transform orientation; //��������и�orientation��������ʶ���ƶ�����

    public PlayerClimb pc;

    //�����Ӱ˵ı���
    float horizontalMovement;
    float verticalMovement;
    Vector3 moveDirection;
    Rigidbody rb;

    [Header("Camera Effects")]
    public PlayerCam cam;

    public float grappleFOV = 120f;
    private float camFov;

    [Header("����˶�״̬")]
    public MoveState state;

    public bool wallRunning;
    public bool climbing;
    public bool freeze;
    public bool unlimited;
    public bool restricted;

    public bool activeGrapple;

    //����˶�״̬
    public enum MoveState
    {
        freeze,
        grappling,
        unlimited,
        Walking,
        Running,
        wallRunning,
        climbing,
        crouching,
        Jumping
    }

    //״̬����
    private void StateHandle()
    {
        //����״̬
        if (freeze)
        {
            state = MoveState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        else if (activeGrapple)
        {
            state = MoveState.grappling;
            moveSpeed = runSpeed;
        }
        //�����ٶ�״̬
        else if (unlimited)
        {
            state = MoveState.unlimited;
            moveSpeed = 999f;
            return;
        }
        //��ǽ״̬
        else if (climbing)
        {
            state = MoveState.climbing;
            moveSpeed = climbSpeed;
        }

        //ǽ���ܲ�״̬
        else if (wallRunning)
        {
            state = MoveState.wallRunning;
            moveSpeed = wallRunSpeed;
        }
        //�¶�״̬
        else if (Input.GetKey(crouchKey))
        {
            state = MoveState.crouching;
            moveSpeed = crouchMoveSpeed;
        }

        //���״̬
        else if (isGrounded && Input.GetKey(runKey))
        {
            state = MoveState.Running;
            moveSpeed = runSpeed;
        }
        //����״̬
        else if (isGrounded)
        {
            state = MoveState.Walking;
            moveSpeed = walkSpeed;
        }
        //��Ծ״̬
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
        //������
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        PlayerInput();
        SpeedControl();
        StateHandle();
        //����Ħ����
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

    //�������
    private void PlayerInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        //��Ծ
        if (Input.GetKeyDown(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); //������Ծcd
        }

        //�¶�
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYscale, transform.localScale.z);
            rb.AddForce(Vector3.down * crouchSpeed, ForceMode.Impulse);
        }
        //վ��
        else if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
        }
    }


    //����ƶ�
    private void PlayerMove()
    {
        //�������ʹ�ù�צ����Ҫ�ƶ�
        if (activeGrapple)
        {
            return;
        }

        //������ᣬ��Ҫ�ƶ�
        if (restricted)
        {
            return;
        }

        //���������ǽ����Ҫ�ƶ�
        if (pc.exitingWall)
        {
            return;
        }

        //�����ƶ�����
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
        //б��
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        //����
        else if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        //����
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airDrag, ForceMode.Force);
        }

        if (!wallRunning)
        {
            rb.useGravity = !OnSlope(); //��б�µ�ʱ��ر�����
        }
    }

    //��������ڲ�ͬ����µ��ٶ�
    private void SpeedControl()
    {
        //�������ʹ�ù�צ����Ҫ�����ٶ�
        if (activeGrapple)
        {
            return;
        }

        //б���ϵ��ƶ��ٶ�
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        //����Ϳ��е��ƶ��ٶ�
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //�����ٶ�
            //�����ƶ��ٶ�
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    //��Ծ
    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); //ִ��һ��һ�����ϵ���
    }

    //����������
    private void ApplyDownForce()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * downForce, ForceMode.Force);
        }
    }

    //������Ծ
    private void ResetJump()
    {
        exitingSlope = false;
        readyToJump = true;
    }

    //�������Ƿ���б����
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            //Debug.Log("б�½Ƕȣ�" + slopeAngle);
            return slopeAngle < maxSlopeAngle && slopeAngle != 0;
        }

        return false;
    }

    //��ȡб���ƶ�����
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    //���㹳����Ծ�ٶ�
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

    //��������ָ��λ��
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3.5f);
    }

    private Vector3 velocityToSet;


    private bool enableMovementOnNextTouch;

    //���ù����ٶ�
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
        cam.DoFov(grappleFOV);
    }

    //���ù���
    public void ResetRestrictions()
    {
        activeGrapple = false;
        cam.DoFov(camFov);
    }


    //�ָ��ƶ�
    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            GetComponent<PlayerGrappling>().StopGrapple();
        }
    }
}