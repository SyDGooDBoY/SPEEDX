using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Animator doorAnimator;  // �����ŵ� Animator ���
    public string openTriggerName = "Open";  // Animator �еĴ���������
    public Collider triggerCollider;  // ���봥��������� Collider

    void Start()
    {
        // ȷ���������� Collider ������Ϊ isTrigger
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // ����Ƿ�Ϊ��ҽ��봥��������
        {
            doorAnimator.SetTrigger(openTriggerName);  // �������Ŷ���
        }
    }
}
