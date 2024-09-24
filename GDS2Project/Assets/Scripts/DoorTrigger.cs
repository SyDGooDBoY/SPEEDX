using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Animator doorAnimator;  // 拖入门的 Animator 组件
    public string openTriggerName = "Open";  // Animator 中的触发器名称
    public Collider triggerCollider;  // 拖入触发器对象的 Collider

    void Start()
    {
        // 确保触发器的 Collider 已设置为 isTrigger
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // 检查是否为玩家进入触发器区域
        {
            doorAnimator.SetTrigger(openTriggerName);  // 触发开门动画
        }
    }
}
