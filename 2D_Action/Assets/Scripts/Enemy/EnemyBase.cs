using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField]
    private float hp;

    [SerializeField]
    private float atkPow;

    [SerializeField]
    private float moveSpeed;

    public float HP
    {
        get => hp;
        set 
        { 
            hp = value; 
        }
    }

    public enum BehaviorState : byte
    {
        Idle = 0,   // 대기상태
        Attack,     // 공격상태
        Dead        // 사망상태
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }

    void Update_Idle()
    {

    }

    void Update_Attack()
    {

    }

    void Update_Dead()
    {

    }

    /// <summary>
    /// 플레이어를 공격하는 함수
    /// </summary>
    void Attack()
    {

    }

}
