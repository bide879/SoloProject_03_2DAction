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
        Turn,       // 방향전환상태
        Attack,     // 공격상태
        Dead        // 사망상태
    }

    /// <summary>
    /// 적의 현재 상태
    /// </summary>
    BehaviorState state = BehaviorState.Idle;

    /// <summary>
    /// 적의 상태 확인 및 설정용 프로퍼티
    /// </summary>
    BehaviorState State
    {
        get => state;
        set
        {
            if (state != value)          // 상태가 달라지면
            {
                state = value;
            }
        }
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
    private Transform EnemyHPBarBG;
    private Transform BulletSpowner;

    private float enemyHPBar;

    /// <summary>
    /// 적이 가지고있는 마크 갯수
    /// </summary>
    public int markCount = Mathf.Clamp(0,0,3);

    private Animator animator;

    private Rigidbody2D rigid;

    private float forward = 1;

    //readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int OnTurnHash = Animator.StringToHash("OnTurn");
    readonly int OnAttackHash = Animator.StringToHash("OnAttack");
    readonly int OnHitHash = Animator.StringToHash("OnHit");
    readonly int OnDieHash = Animator.StringToHash("OnDie");

    private void Awake()
    {
        EnemyHPBarBG = transform.GetChild(1);
        EnemyHPBar = transform.GetChild(2);
        BulletSpowner = transform.GetChild(3);

        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        EnemyHPBarBG.gameObject.SetActive(false);
        EnemyHPBar.gameObject.SetActive(false);
    }
    private void Update()
    {
        Debug.DrawRay(rigid.position + Vector2.down * 0.5f, Vector3.right * -3.0f * forward, new Color(0, 1, 0));
        RaycastHit2D rayBackHit = Physics2D.Raycast(rigid.position + Vector2.down * 0.5f, Vector3.right * forward, -3, LayerMask.GetMask("Player"));

        Debug.DrawRay(rigid.position + Vector2.down * 0.5f, Vector3.right * 8.0f * forward, new Color(1, 0, 0));
        RaycastHit2D rayAttackHit = Physics2D.Raycast(rigid.position + Vector2.down * 0.5f * forward, Vector3.right, 8, LayerMask.GetMask("Player"));

        if (rayBackHit.collider != null)
        {
            Update_Turn();
        }
        else if (rayAttackHit.collider != null)
        {
            Update_Attack();
        }
        else 
        {
            Update_Idle();
        }
    }

    void Update_Idle()
    {
        State = BehaviorState.Idle;
    }

    void Update_Attack()
    {
        if(State != BehaviorState.Turn)
        {
            StartCoroutine(OnUpdateCoolTime(0.5f));
            State = BehaviorState.Attack;
            animator.SetTrigger(OnAttackHash);
        }
    }

    public void OnFire()
    {
        Factory.Instance.GetSpownEnemy01_Bullet(BulletSpowner.position, transform.rotation);
    }

    void Update_Turn()
    {
        if(State != BehaviorState.Turn)
        {
            StartCoroutine(OnTurnCoolTime());
        }
    }

    void Update_Dead()
    {

    }

    private IEnumerator OnUpdateCoolTime(float time)
    {
        yield return new WaitForSeconds(time);
    }

    private IEnumerator OnTurnCoolTime()
    {
        State = BehaviorState.Turn;
        //yield return new WaitForSeconds(1f);
        forward = -forward;
        animator.SetTrigger(OnTurnHash);
        yield return new WaitForSeconds(0.45f);
        transform.localScale = new Vector3(forward, 1, 1);
        yield return new WaitForSeconds(2f);
        State = BehaviorState.Idle;
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
        if (EnemyHPBar.gameObject)
        {
            EnemyHPBarBG.gameObject.SetActive(true);
            EnemyHPBar.gameObject.SetActive(true);
        }
        if (IsAlive)
        {
            //Time.timeScale = 0.1f;

            animator.SetTrigger(OnHitHash);

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
        if (EnemyHPBar.gameObject)
        {
            EnemyHPBarBG.gameObject.SetActive(false);
            EnemyHPBar.gameObject.SetActive(false);
        }
        onDie?.Invoke();                // 죽었다고 알림 보내기
        onDie = null;                   // 죽으면 onDie도 초기화
        StartCoroutine(OnDieAnimation());
        State = BehaviorState.Dead;
    }

    private IEnumerator OnDieAnimation()
    {
        animator.SetTrigger(OnDieHash);
        yield return new WaitForSeconds(1.0f);
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
