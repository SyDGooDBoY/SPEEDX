using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCam : MonoBehaviour
{
    public Transform orientationPlayer; //���ٶ�����Եķ���
    [Header("XY������")] public float sensX;
    public float sensY;
    [Header("��ת�Ƕ�")] float rotationY;
    float rotationX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90, 90);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientationPlayer.rotation = Quaternion.Euler(0, rotationY, 0);
    }
}