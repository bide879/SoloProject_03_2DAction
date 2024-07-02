using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_01 : EnemyBase
{
    private Transform BulletSpowner;

    private int attackType = 0;

    public enum PhaseState : byte
    {
        Phase1 = 0,   // 1페이즈 체력 100% ~ 66%
        Phase2,       // 2페이즈 체력 66% ~ 33%
        Phase3,     // 3페이즈 체력 33% ~
    }

    PhaseState phase = PhaseState.Phase1;

    /// <summary>
    /// 보스 페이즈 확인 및 설정용 프로퍼티
    /// </summary>
    PhaseState Phase
    {
        get => phase;
        set
        {
            if (phase != value)          // 상태가 달라지면
            {
                phase = value;
            }
        }
    }

    readonly int OnUltimateHash = Animator.StringToHash("OnUltimate");
    readonly int OnNextPhaseHash = Animator.StringToHash("OnNextPhase");
    readonly int OnAppearEndHash = Animator.StringToHash("OnAppearEnd"); 
    readonly int AttackTypeHash = Animator.StringToHash("AttackType");


    protected override void Awake()
    {
        BulletSpowner = transform.GetChild(2);
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        isBoss = true;
    }

    protected override void Start()
    {
        Phase = PhaseState.Phase1;
        Update_Idle();
    }
    protected override void Update()
    {
        //
    }

    protected override void EnemyRay()
    {
        Debug.DrawRay(rigid.position + Vector2.down * 0.5f, Vector3.right * -5.0f * forward, new Color(0, 1, 0));
        rayBackHit = Physics2D.Raycast(rigid.position + Vector2.down * 0.5f, Vector3.right * forward, -5.0f, LayerMask.GetMask("Player"));
    }

    protected override void Update_Idle()
    {
        base.Update_Idle();
        float randamTime = Random.Range(1.2f, 2.4f);
        StartCoroutine(OnUpdateCoolTime(randamTime));
        StartCoroutine(attackCo());
    }

    IEnumerator attackCo()
    {
        yield return new WaitForSeconds(0.5f);
        Update_Attack();
    }

    protected override void Update_Attack()
    {
        attackType = Random.Range(0, 4);
        switch (attackType)
        {
            case 0:
                animator.SetTrigger(OnAttackHash);
                animator.SetInteger(AttackTypeHash, attackType);            
                Move();
                Update_Idle();
                break;
            case 1:
                animator.SetTrigger(OnAttackHash);
                animator.SetInteger(AttackTypeHash, attackType);
                Update_Idle();
                break;
            case 2:
                animator.SetTrigger(OnAttackHash);
                animator.SetInteger(AttackTypeHash, attackType);
                Update_Idle();
                break;
            case 3:
                animator.SetTrigger(OnAttackHash);
                animator.SetInteger(AttackTypeHash, attackType);
                Update_Idle();
                break;
        }
    }

    protected override void Update_Turn()
    {
        
    }

    protected override void Update_Dead()
    {
        
    }

    private void Move()
    {
        rigid.AddForce(Vector2.zero);
        rigid.AddForce(new Vector2(-forward * 8, 10), ForceMode2D.Impulse);
    }

    private void Attack1()
    {

    }

    private void Attack3()
    {

    }

    public void OnFireAttack01()
    {
        Factory.Instance.GetSpownBossAttack01Bullet(BulletSpowner.position, forward, attackPower * 0.8f);
    }

    public void OnFireAttack03()
    {
        Factory.Instance.GetSpownBossAttack03Bullet(BulletSpowner.position, forward, attackPower);
    }

    public void OnFireAttack04()
    {
        Factory.Instance.GetSpownBossAttack04Bullet(BulletSpowner.position, forward, attackPower);
    }

    private void NextPhase()
    {

    }

}

