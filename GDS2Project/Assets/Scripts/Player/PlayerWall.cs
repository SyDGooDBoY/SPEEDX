using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerWall : MonoBehaviour
{
    [Header("ǽ�ڼ��")]
    public float wallDistance = 0.5f; //ǽ�ھ���

    public float minimumJumpHeight = 1.5f; //��С��Ծ�߶�

    public LayerMask wallMask; //ǽ�ڲ�
    public LayerMask groundMask; //�����
    private RaycastHit leftWallHit; //��ǽ���
    private RaycastHit rightWallHit; //��ǽ���
    private bool isWallRight; //��ǽ
    private bool isWallLeft; //��ǽ

    [Header("�˳�ǽ��")]
    private bool exitingWall; //�˳�ǽ��

    public float exitWallTime = 0.2f; //�˳�ǽ��ʱ��
    private float exitWallTimer; //�˳�ǽ�ڼ�ʱ��

    [Header("����")]
    public bool useGravity;

    [Tooltip("������������")]
    public float gravityCounterForce; //������������


    [Header("�����ǽ�ϵ��ƶ�����")]
    public float wallRunSpeed = 10f; //ǽ���ƶ��ٶ�

    public float wallClimbSpeed = 5f; //ǽ�������ٶ�
    public float WallJumpUpForce = 10f; //ǽ����Ծ��
    public float WallJumpSideForce = 70f; //ǽ����Ծ������
    public float wallRunMaxTime = 2f; //ǽ�����ʱ��
    private float wallRunTimer; //ǽ��ʱ��

    [Header("�������")]
    public KeyCode jumpKey = KeyCode.Space; //��Ծ

    public KeyCode upwardsRunKey = KeyCode.LeftShift; //ǽ��������
    public KeyCode downwardRunKey = KeyCode.LeftControl; //ǽ��������
    private bool upwardsRunning; //������
    private bool downwardRunning; //������

    private float horizontalInput; //ˮƽ����

    private float verticalInput; //��ֱ����

    [Header("�ο�����")]
    [Tooltip("��������и�orientation��������ʶ���ƶ�����")]
    public Transform orientation; //��������и�orientation��������ʶ���ƶ�����

    public PlayerCam cam; //�����

    public float onWallFOV = 120f; //ǽ����Ұ

    // private PlayerGrab pg;
    private float camFov; //�������Ұ
    private PlayerMovement playerMovement; //����ƶ��ű�
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

    //���ǽ��
    private void WallCheck()
    {
        isWallLeft =
            Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance, wallMask); //��ǽ
        isWallRight =
            Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance, wallMask); //��ǽ
    }

    //����Ƿ��ڵ����Ϸ�
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight, groundMask);
    }

    //ǽ�ڽ�����״̬
    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardRunning = Input.GetKey(downwardRunKey);
        //ǽ����
        if ((isWallLeft || isWallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            //��ǽ����
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

            //ǽ����Ծ
            if (Input.GetKeyDown(jumpKey))
            {
                WallJump();
            }
        }

        //�뿪ǽ��
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

    //��ʼǽ����
    private void StartWallRun()
    {
        playerMovement.wallRunning = true;
        wallRunTimer = wallRunMaxTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //Ӧ�������Ч��
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

    //ǽ���ƶ�
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

        //ǽ����ǰ��
        rb.AddForce(wallForward * wallRunSpeed, ForceMode.Force);

        //ǽ�����ϻ���������
        if (upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }

        if (downwardRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }

        //��ǽ����
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
        //���� y ���ٶȲ������Ծ��
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        playerMovement.canControlInAir = true;
    }
}