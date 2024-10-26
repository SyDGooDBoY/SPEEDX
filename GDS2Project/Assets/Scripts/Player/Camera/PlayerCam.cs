using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class PlayerCam : MonoBehaviour
{
    public Transform orientationPlayer; // ���ٶ�����Եķ���
    public Transform camHolder; // ������ĸ�����

    [Header("��ת�ٶ�")]
    public float rotationSpeed = 1.0f; // �����������ת���ٶ�

    float rotationY;
    float rotationX;

    private PlayerMovement pm;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!pm.inputEnabled) return;
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90, 90);

        // ʹ�ò�ֵƽ����ת
        Quaternion targetRotationCam = Quaternion.Euler(rotationX, rotationY, 0);
        Quaternion targetRotationOrientation = Quaternion.Euler(0, rotationY, 0);

        camHolder.localRotation =
            Quaternion.Lerp(camHolder.localRotation, targetRotationCam, Time.deltaTime * 45); // �ɵ�����ֵ�ٶ�
        orientationPlayer.rotation =
            Quaternion.Slerp(orientationPlayer.rotation, targetRotationOrientation,
                Time.deltaTime * 30); // ʹ��Slerp�Ա��ָ���Ȼ����ת
    }

    // apply it when respawn
    public void SetCameraToLookAtTarget(Transform target)
    {

        camHolder.LookAt(target.position);

        Vector3 directionToTarget = target.position - camHolder.position;
        Debug.Log(camHolder.position + "7777");

        rotationY = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        float distanceToTarget = new Vector2(directionToTarget.x, directionToTarget.z).magnitude;
        rotationX = Mathf.Atan2(directionToTarget.y, distanceToTarget) * Mathf.Rad2Deg;

        camHolder.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientationPlayer.rotation = Quaternion.Euler(0, rotationY, 0);
    }


    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f).SetEase(Ease.InOutQuad); // ��ӻ���������ƽ������
    }

    public void DoFovDash(float endValue, float duration)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, duration).SetEase(Ease.InOutQuad); // ��ӻ���������ƽ������
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f).SetEase(Ease.InOutQuad); // ͬ��ʹ�û���
    }
}