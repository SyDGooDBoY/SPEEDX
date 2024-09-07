using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWall : MonoBehaviour
{
    [Header("墙壁检测")]
    public LayerMask wallMask; //墙壁层
    public LayerMask groundMask; //地面层
    [Header("玩家在墙上的移动属性")]
    public float wallDistance = 0.5f; //墙壁距离
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
