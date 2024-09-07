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


    [Header("������")]
    //��
    bool isGrounded;

    public float playerHeight; //��Ҹ߶�

    public LayerMask groundMask; //�����

    [Header("б���ƶ�")]
    public float maxSlopeAngle = 40f; //���б�½Ƕ�

    private RaycastHit slopeHit; //б�¼��
    private bool exitingSlope; //�˳�б��

    [Header("ʶ���ƶ�����Ķ���")]
    [Tooltip("��������и�orientation��������ʶ���ƶ�����")]
    public Transform orientation; //��������и�orientation��������ʶ���ƶ�����

    //�����Ӱ˵ı���
    float horizontalMovement;
    float verticalMovement;
    Vector3 moveDirection;
    Rigidbody rb;

    [Header("����˶�״̬")]
    public MoveState state;

    //����˶�״̬
    public enum MoveState
    {
        Walking,
        Running,
        crouching,
        Jumping
    }

    //״̬����
    private void StateHandle()
    {
        //�¶�״̬
        if (Input.GetKey(crouchKey))
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

        rb.useGravity = !OnSlope(); //��б�µ�ʱ��ر�����
    }

    //��������ڲ�ͬ����µ��ٶ�
    private void SpeedControl()
    {
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
            Debug.Log("б�½Ƕȣ�" + slopeAngle);
            return slopeAngle < maxSlopeAngle && slopeAngle != 0;
        }

        return false;
    }

    //��ȡб���ƶ�����
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}