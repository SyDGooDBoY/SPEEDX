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

    private PlayerDash pd;
    public AudioSource audioSource;

    public AudioClip wallRunSound;

    // Start is called before the first frame update
    void Start()
    {
        // pg = GetComponent<PlayerGrab>();
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        pd = GetComponent<PlayerDash>();
        cam = GameObject.Find("Camera").GetComponent<PlayerCam>();

        camFov = cam.GetComponent<Camera>().fieldOfView;
        cam.DoFov(camFov);
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        wallRunSound = Resources.Load<AudioClip>("Sound/NEW SOUNDS/WALLRUN");
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
        // upwardsRunning = Input.GetKey(upwardsRunKey);
        // downwardRunning = Input.GetKey(downwardRunKey);
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
        pd.dashCdTimer = 0;
        playerMovement.wallRunning = true;
        wallRunTimer = wallRunMaxTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //Ӧ�������Ч��
        cam.DoFov(onWallFOV);
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.clip = wallRunSound;
        audioSource.Play();
        if (isWallLeft)
        {
            cam.DoTilt(-10f);
        }

        if (isWallRight)
        {
            cam.DoTilt(10f);
        }
    }

    //ǽ���ƶ�
    private void WallRunMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunSpeed, ForceMode.Force);

        // Adjust vertical velocity based on camera pitch
        float cameraPitch = cam.transform.eulerAngles.x;
        if (cameraPitch > 180) // Adjust for Unity's 360-degree system
        {
            cameraPitch -= 360;
        }

        // Move up if looking up, down if looking down
        float verticalSpeed = cameraPitch < 0 ? wallClimbSpeed : -wallClimbSpeed;
        rb.velocity = new Vector3(rb.velocity.x, verticalSpeed, rb.velocity.z);

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
        audioSource.Stop();
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