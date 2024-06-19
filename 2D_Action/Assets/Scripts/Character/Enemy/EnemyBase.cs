using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IBattler, IHealth
{
    /// <summary>
    /// HP
    /// </summary>
    [SerializeField]
    protected float hp = 100.0f;

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
            if (hp <= 0.0f)    // HP가 0 이하면 사망처리
            {
                Die();
            }
            enemyHPBar = Mathf.Clamp(hp/100,0,1);
        }
    }

    public enum BehaviorState : byte
    {
        Idle = 0,   // 대기상태
        Attack,     // 공격상태
        Dead        // 사망상태
    }

    /// <summary>
    /// 최대 HP(변수는 인스펙터에서 수정하기 위해 public으로 만든 것임)
    /// </summary>
    public float maxHP = 100.0f;
    public float MaxHP => maxHP;

    /// <summary>
    /// HP 변경시 실행되는 델리게이트
    /// </summary>
    public Action<float> onHealthChange { get; set; }

    /// <summary>
    /// 살았는지 죽었는지 확인하기 위한 프로퍼티
    /// </summary>
    public bool IsAlive => hp > 0;

    /// <summary>
    /// 이 캐릭터가 죽었을 때 실행되는 델리게이트
    /// </summary>
    public Action onDie { get; set; }

    /// <summary>
    /// 이 캐릭터가 맞았을 때 실행되는 델리게이트(int : 실제로 입은 데미지)
    /// </summary>
    public Action<int> onHit { get; set; }

    /// <summary>
    /// 공격력(변수는 인스펙터에서 수정하기 위해 public으로 만든 것임)
    /// </summary>
    public float attackPower = 10.0f;
    public float AttackPower => attackPower;

    /// <summary>
    /// 방어력(변수는 인스펙터에서 수정하기 위해 public으로 만든 것임)
    /// </summary>
    public float defencePower = 0.0f;
    public float DefencePower => defencePower;

    /// <summary>
    /// 공격 대상
    /// </summary>
    protected IBattler attackTarget = null;

    private Transform EnemyHPBar;

    private float enemyHPBar;

    /// <summary>
    /// 적이 가지고있는 마크 갯수
    /// </summary>
    public int markCount = Mathf.Clamp(0,0,3);

    private void Awake()
    {
        EnemyHPBar = transform.GetChild(2);
        //EnemyHPBar = GetComponent<Transform>();
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
    /// 공격처리용 함수
    /// </summary>
    /// <param name="target">공격 대상</param>
    public void Attack(IBattler target)
    {
        target.Defence(AttackPower);
    }

    /// <summary>
    /// 방어 처리용 함수
    /// </summary>
    /// <param name="damage">내가 받은 순수 데미지</param>
    public void Defence(float damage)
    {
        if (IsAlive)
        {
            //Time.timeScale = 0.1f;

            //animator.SetTrigger(OnHitHash);

            float final = Mathf.Max(0, damage - DefencePower);  // 0 이하로는 데미지가 내려가지 않는다.
            HP -= final;

            EnemyHPBar.localScale = new Vector3 (enemyHPBar,1,1);
            onHit?.Invoke(Mathf.RoundToInt(final));
        }
    }

    /// <summary>
    /// 사망 처리용 함수
    /// </summary>
    public void Die()
    {
        onDie?.Invoke();                // 죽었다고 알림 보내기
        onDie = null;                   // 죽으면 onDie도 초기화
        gameObject.SetActive(false);
    }

    public void HealthRegenerate(float totalRegen, float duration)
    {
        throw new NotImplementedException();
    }

    public void HealthRegenerateByTick(float tickRegen, float tickInterval, uint totalTickCount)
    {
        throw new NotImplementedException();
    }

}
