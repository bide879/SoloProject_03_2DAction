using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IBattler, IHealth
{
    #region 변수 선언

    // -------------- 이동 관련 변수 --------------

    /// <summary>
    /// 이동속도
    /// </summary>
    private float moveSpeed = 5.0f;
    public float MoveSpeed => moveSpeed;

    /// <summary>
    /// 보통 이동속도
    /// </summary>
    private float normalSpeed;

    /// <summary>
    /// 대시 속도
    /// </summary>
    private float dashSpeed = 8.0f;
    public float DashSpeed => dashSpeed;

    /// <summary>
    /// 현재 입력된 이동 방향
    /// </summary>
    private Vector2 inputDirection = Vector2.zero;

    // -------------- 점프 관련 변수 --------------

    /// <summary>
    /// 점프 높이
    /// </summary>
    private float jumpPower = 10.0f;
    public float JumpPower => jumpPower;

    /// <summary>
    /// 점프 횟수(2단점프 확인 및 초기화 용)
    /// </summary>
    private int jumpCount = 0;

    /// <summary>
    /// 점프공격 가능 여부
    /// </summary>
    bool canJumpAttack = true;
    private float pushJumpTime = 0f;
    private float maxJumpPower = 10.0f;
    private float minJumpPower = 5.0f;
    private bool jumpButtonPressed = false;


    /// <summary>
    /// 지상/공중 여부
    /// </summary>
    private bool isGrounded = true;

    // -------------- 전투 관련 변수 --------------

    /// <summary>
    /// 연속으로 Down 키를 눌러야 하는 횟수
    /// </summary>
    private const int requiredPressCount = 2;

    /// <summary>
    /// Down 키를 누른 횟수
    /// </summary>
    private int moveDownPressCount = 0;

    // 인풋액션
    PlayerInputActions inputActions;
    Rigidbody2D rigid;
    Animator animator;
    Ghost ghost;

    public bool isAttackPush = false;
    public bool IsAttackPush => isAttackPush;


    private bool canAttack = true;
    private float attackDelay = 0.16f;
    private Vector2 previousInputDirection;
    private float previousMoveSpeed;
    private bool isAirDownAttack = false;

    public Action<int> SkillCostChang;
    private int skillCost = 5;


    private int SkillCost
    {
        get => skillCost;
        set
        {
            skillCost = Mathf.Clamp(value, 0, maxSkillCost); ;
        }
    }
    private int maxSkillCost = 5;



    /// <summary>
    /// 플레이어의 현재 HP
    /// </summary>
    float hp = 100.0f;
    public float HP
    {
        get => hp;
        set
        {
            if (IsAlive)     // 살아 있을 때 확인
            {
                hp = value;
                if (hp <= 0.0f)    // HP가 0 이하면 사망처리
                {
                    Die();
                }
                hp = Mathf.Clamp(hp, 0, MaxHP);     // 최소~최대 사이로 숫자 유지
                onHealthChange?.Invoke(hp / MaxHP);   // 델리게이트로 HP변화 알림
                //Debug.Log($"{this.gameObject.name} HP : {hp}");
            }
        }
    }

    /// <summary>
    /// 플레이어의 최대 HP
    /// </summary>
    float maxHP = 100.0f;
    public float MaxHP => maxHP;

    /// <summary>
    /// HP의 변경을 알리는 델리게이트
    /// </summary>
    public Action<float> onHealthChange { get; set; }

    /// <summary>
    /// 플레이어의 생존 여부를 확인하기 위한 프로퍼티
    /// </summary>
    public bool IsAlive => hp > 0;

    /// <summary>
    /// 플레이어의 사망을 알리는 델리게이트
    /// </summary>
    public Action onDie { get; set; }

    // 플레이어의 공격력과 방어력
    public float baseAttackPower = 10.0f;
    public float baseDefencePower = 1.0f;
    float attackPower = 10.0f;
    public float AttackPower => attackPower;
    float defencePower = 1.0f;
    public float DefencePower => defencePower;

    /// <summary>
    /// 이 캐릭터가 맞았을 때 실행되는 델리게이트(int : 실제로 입은 데미지)
    /// </summary>
    public Action<int> onHit { get; set; }

    /// <summary>
    /// 로딩 부르는 델리게이트
    /// </summary>
    public Action onLoding;

    // 애니메이터용 해시값
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int IsJumpHash = Animator.StringToHash("IsJump");
    readonly int OnAttackHash = Animator.StringToHash("OnAttack");
    readonly int IsAttackPushHash = Animator.StringToHash("IsAttackPush");
    readonly int OnDownAttackHash = Animator.StringToHash("OnDownAttack");
    readonly int OnSkillHash = Animator.StringToHash("OnSkill");
    readonly int OnChargeEnergyHash = Animator.StringToHash("OnChargeEnergy");
    readonly int OnHitHash = Animator.StringToHash("OnHit");
    readonly int OnDieHash = Animator.StringToHash("OnDie");
    readonly int OnResurrectionHash = Animator.StringToHash("OnResurrection");

    #endregion

    private void Awake()
    {
        inputActions = new PlayerInputActions(); // 인풋 액션 생성
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        ghost = GetComponentInChildren<Ghost>();
    }

    private void Start()
    {
        normalSpeed = moveSpeed;
        ghost.makeGhost = false;
        isResurrection = false;
    }

    #region 플레이어 조작

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStop;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Jump.canceled += OnJump;
        inputActions.Player.Dash.performed += OnDash;
        inputActions.Player.Dash.canceled += OnDash;
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Skill.performed += OnSkill;
        inputActions.Player.Ultimate.performed += OnUltimate;
    }

    private void OnDisable()
    {
        inputActions.Player.Ultimate.performed -= OnUltimate;
        inputActions.Player.Skill.performed -= OnSkill;
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Dash.canceled -= OnDash;
        inputActions.Player.Dash.performed -= OnDash;
        inputActions.Player.Jump.canceled -= OnJump;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Move.canceled -= OnStop;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        // 점프 버튼을 누르고 있는 시간 계산
        if (jumpButtonPressed)
        {
            pushJumpTime += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (isResurrection)
        {
            hp += MaxHP;
            skillCost += maxSkillCost;
        }

        if (IsAlive)
        {
            float h = Input.GetAxisRaw("Horizontal");
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);


            if (rigid.velocity.x > moveSpeed) // right max speed
            {
                rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
            }
            else if (rigid.velocity.x < moveSpeed * (-1))// left max speed
            {
                rigid.velocity = new Vector2(moveSpeed * (-1), rigid.velocity.y);
            }


            if (rigid.velocity.y < 0) //내려갈떄만 스캔
            {
                Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
                RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Ground"));
                if (rayHit.collider != null)
                {
                    if (rayHit.distance < 1f)
                    {
                        animator.SetBool(IsJumpHash, false);
                        if (!isGrounded)
                        {
                            StartCoroutine(GraduallyReduceSpeed());
                        }
                        if (isAirDownAttack)
                        {
                            rigid.gravityScale = 2f;
                            SkillCost = maxSkillCost;
                            SkillCostChang?.Invoke(SkillCost);
                            //StartCoroutine(HandleChargeEnergy());
                            isAirDownAttack = false;
                        }
                        isGrounded = true;
                        canJumpAttack = true;
                        jumpCount = 0;
                        //inputDirection = Vector2.zero;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 방향키를 눌렀을때 실행될 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            inputDirection = Vector2.zero;
        }
        inputDirection = context.ReadValue<Vector2>();

        if (context.phase == InputActionPhase.Performed && context.ReadValue<Vector2>().y < 0)
        {
            if (isGrounded)
            {
                moveDownPressCount++;
            }

            if (moveDownPressCount >= requiredPressCount && isGrounded)
            {
                OnChargeEnergy();
                moveDownPressCount = 0; // 초기화
            }
        }
        else
        {
            if (inputDirection.x != 0)
            {
                animator.SetBool(IsMoveHash, true);
                if (inputDirection.x < 0)
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                }
                else
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                }

            }
            else
            {
                animator.SetBool(IsMoveHash, false);
            }

            moveDownPressCount = 0;
        }
    }

    /// <summary>
    /// 동작을 멈췄을 때 실행할 함수
    /// </summary>
    /// <param name="_"></param>
    private void OnStop(InputAction.CallbackContext _)
    {
        inputDirection = Vector2.zero;
        animator.SetBool(IsMoveHash, false);
    }

    /// <summary>
    /// 점프 버튼을 눌렀을 때 실행할 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (rigid.velocity.y > 3.5f) // 상승 중일 때만
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0); // y 축 속도를 0으로 설정하여 상승 멈추기
                rigid.AddForce(Vector2.zero);
                rigid.AddForce(Vector2.up * minJumpPower, ForceMode2D.Impulse);
            }
        }
        else if (context.performed)
        {
            jumpButtonPressed = true;
            pushJumpTime = 0f; // 점프 버튼을 누르기 시작할 때 초기화
            Jump(maxJumpPower);
            moveDownPressCount = 0;
            jumpButtonPressed = false;
        }
    }

    /// <summary>
    /// 점프/2단점프 처리하는 함수
    /// </summary>
    /// <param name="jumpPower">점프 힘</param>
    private void Jump(float jumpPower)
    {
        if (!isGrounded && jumpCount < 2)
        {
            if (rigid.velocity.y < 5 && !isAirDownAttack)
            {
                ResetGravity();
                rigid.AddForce(Vector2.zero);
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                animator.SetBool(IsJumpHash, true);
                isGrounded = false;
                canJumpAttack = true;
                jumpCount++;
            }
        }
        else if (isGrounded)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool(IsJumpHash, true);
            isGrounded = false;
            canJumpAttack = true;
            jumpCount++;
        }

        // 점프가 끝나면 누르고 있는 시간 초기화
        pushJumpTime = 0f;
    }

    /// <summary>
    /// 대시 버튼을 눌렀을 때 실행될 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnDash(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            if (isGrounded)
            {
                if (inputDirection.x == 0 || chargeEnergy || isAirDownAttack)
                {
                    return;
                }
                Factory.Instance.GetSpownDashEffect(this.transform.position, this.transform.rotation);
                StopAllCoroutines();
                rigid.AddForce((Vector2.right * inputDirection.x) * 15.0f, ForceMode2D.Impulse);
                moveSpeed = dashSpeed;
                ghost.makeGhost = true;
            }
        }
        else
        {
            if (isGrounded)
            {
                StartCoroutine(GraduallyReduceSpeed());
            }
        }
    }

    /// <summary>
    /// 대시 종료시 처리를 위한 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator GraduallyReduceSpeed()
    {
        float duration = 0.5f; // 속도를 줄이는 데 걸리는 시간
        float elapsed = 0f;

        while (elapsed < duration)
        {
            moveSpeed = Mathf.Lerp(dashSpeed, normalSpeed, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        ghost.makeGhost = false;
        moveSpeed = normalSpeed;
    }

    /// <summary>
    /// 중력을 초기화하는 함수
    /// </summary>
    private void ResetGravity()
    {
        // 중력 가속도를 초기화 (y 방향 속도를 0으로 설정)
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
    }


    // ---------------------------------------- 공격 -----------------------------------------

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (canAttack && canJumpAttack)
        {
            if (isGrounded)
            {
                // 공격 시작 전 이동을 멈춤
                previousInputDirection = inputDirection;
                if (isGrounded)
                {
                    previousMoveSpeed = moveSpeed;
                    moveSpeed = 0.0f;
                }
                inputDirection = Vector2.zero;
                animator.SetBool(IsMoveHash, false);
            }
            animator.SetBool(IsAttackPushHash, isAttackPush);
            animator.SetTrigger(OnAttackHash);
            StartCoroutine(HandleAttack());
        }
        if (inputDirection.y < 0 && !isGrounded && !isAirDownAttack)
        {
            AirDownAttack();
        }
    }
    private void AirDownAttack()
    {
        isAirDownAttack = true;
        previousInputDirection = inputDirection;
        previousMoveSpeed = moveSpeed;
        moveSpeed = 0.0f;
        rigid.gravityScale = 0.5f;
        animator.SetTrigger(OnDownAttackHash);
        StartCoroutine(HandleAttack());
    }

    private IEnumerator HandleAttack()
    {
        canAttack = false;
        if (!isGrounded)
        {
            canJumpAttack = false;
        }
        yield return new WaitForSeconds(attackDelay);

        canAttack = true;

        if (isAirDownAttack)
        {
            rigid.gravityScale = 5f;
        }

        // 공격이 끝나면 이전 방향으로 다시 이동
        inputDirection = previousInputDirection;
        if (isGrounded)
        {
            moveSpeed = previousMoveSpeed;
        }
        if (inputDirection.x != 0)
        {
            animator.SetBool(IsMoveHash, true);
        }
    }

    public float skillEffectRotate = 0;

    private void OnSkill(InputAction.CallbackContext context)
    {
        previousInputDirection = inputDirection;
        previousMoveSpeed = moveSpeed;

        if (SkillCost < 1 || isAirDownAttack)
        {
            return;
        }
        animator.SetTrigger(OnSkillHash);

        if (inputDirection.y > 0 && inputDirection.x == 0)
        {
            skillEffectRotate = 90;
        }
        else if (inputDirection.y < 0 && inputDirection.x == 0)
        {
            skillEffectRotate = -90;
        }
        else if (inputDirection.y > 0)
        {
            skillEffectRotate = 30;
        }
        else if (inputDirection.y < 0)
        {
            skillEffectRotate = -30;
        }
        else
        {
            skillEffectRotate = 0;
        }

        if (transform.localScale.x < 0)
        {
            skillEffectRotate = -skillEffectRotate;
        }


        var skillEffect = Factory.Instance.GetSpownSkillEffect(this.transform.position, this.transform.rotation);
        skillEffect.transform.rotation = Quaternion.Euler(0, 0, skillEffectRotate);
        rigid.gravityScale = 0f;
        SkillCost--;
        SkillCostChang?.Invoke(SkillCost);

        ResetGravity();

        if (inputDirection.x == 0 && inputDirection.y != 0)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + previousInputDirection.y * 7);
        }
        else if (inputDirection != Vector2.zero)
        {
            transform.position = new Vector2(transform.position.x + transform.localScale.x * 7, transform.position.y + previousInputDirection.y * 5);
        }
        else
        {
            transform.position = new Vector2(transform.position.x + transform.localScale.x * 7, transform.position.y);
        }
        StartCoroutine(HandleSkill());
    }

    private IEnumerator HandleSkill()
    {
        moveSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f);
        inputDirection = previousInputDirection;
        if(previousMoveSpeed < 0.1f)
        {
            moveSpeed = normalSpeed;
        }
        else
        {
            moveSpeed = previousMoveSpeed;
        }
        rigid.gravityScale = 2f;
        canJumpAttack = true;
        if (jumpCount > 1)
        {
            jumpCount = 1;
        }
    }

    bool chargeEnergy = false;
    private void OnChargeEnergy()
    {

        animator.SetTrigger(OnChargeEnergyHash);
        StartCoroutine(HandleChargeEnergy());
    }

    private IEnumerator HandleChargeEnergy()
    {
        chargeEnergy = true;
        previousInputDirection = inputDirection;
        SkillCost = maxSkillCost;
        previousMoveSpeed = moveSpeed;
        moveSpeed = 0.0f;
        SkillCostChang?.Invoke(SkillCost);

        yield return new WaitForSeconds(0.45f);
        inputDirection = previousInputDirection;
        moveSpeed = previousMoveSpeed;
        chargeEnergy = false;
    }

    private void OnUltimate(InputAction.CallbackContext context)
    {
        //Factory.Instance.GetSpownUltimateEffect();
    }

    #endregion

    #region 플레이어 상호작용


    /// <summary>
    /// 부딛혔을 때 실행할 함수
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsAlive)
        {
            if (collision.tag == "Ground")
            {
                isGrounded = true;
                animator.SetBool(IsJumpHash, false);
                moveSpeed = normalSpeed;
            }
            else if (collision.tag == "EnemyBullet")
            {
                Enemy_01_Bullet enemy01_Bullet = collision.GetComponent<Enemy_01_Bullet>();
                Enemy_02_Bullet enemy02_Bullet = collision.GetComponent<Enemy_02_Bullet>();

                BossAttack01Bullet boss01Bullet = collision.GetComponent<BossAttack01Bullet>();
                BossAttack03Bullet boss03Bullet = collision.GetComponent<BossAttack03Bullet>();
                BossAttack04Bullet boss04Bullet = collision.GetComponent<BossAttack04Bullet>();

                if (enemy01_Bullet != null)
                {
                    playerOnHit(enemy01_Bullet.damage);
                }
                else if (enemy02_Bullet != null)
                {
                    playerOnHit(enemy02_Bullet.damage);
                }else if (boss01Bullet != null)
                {
                    playerOnHit(boss01Bullet.damage);
                }
                else if (boss03Bullet != null)
                {
                    playerOnHit(boss03Bullet.damage);
                }
                else if (boss04Bullet != null)
                {
                    playerOnHit(boss04Bullet.damage);
                }
            }
            else if (collision.tag == "EnemyAttackRange")
            {
                EnemyBase enemy = collision.GetComponentInParent<EnemyBase>();
                if (enemy != null)
                {
                    playerOnHit(enemy.attackPower);
                }
            }
        }
    }

    bool onHitDefence = false;
    Coroutine defenceCoroutine;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsAlive)
        {
            if (collision.tag == "Enemy" && !onHitDefence)
            {
                playerOnHit(10.0f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsAlive)
        {
            if (collision.tag == "Enemy")
            {
                onHitDefence = false;
                if (defenceCoroutine != null)
                {
                    StopCoroutine(defenceCoroutine);
                    defenceCoroutine = null;
                }
            }
        }
    }

    private void playerOnHit(float damege)
    {
        Defence(damege);
        ResetGravity();
        rigid.AddForce(Vector2.zero);
        rigid.AddForce(new Vector2(-inputDirection.x * 8, 3), ForceMode2D.Impulse);

        // 기존 코루틴이 실행 중이라면 중지
        if (defenceCoroutine != null)
        {
            StopCoroutine(defenceCoroutine);
        }

        defenceCoroutine = StartCoroutine(OnHitDefence());
    }

    private IEnumerator OnHitDefence()
    {
        onHitDefence = true;
        yield return new WaitForSeconds(0.5f);
        onHitDefence = false;
        defenceCoroutine = null;
    }

    public void Attack(IBattler target)
    {
        target.Defence(AttackPower);
    }

    public void SkillAttack(IBattler target, int count)
    {
        if(count > 0)
        {
            skillCost = skillCost + count;
            if(skillCost > 5)
            {
                skillCost = 5;
            }
            SkillCostChang?.Invoke(skillCost);
        }
        target.Defence((AttackPower * 3) * (count + 1) );
    }

    public void Defence(float damage)
    {
        if (IsAlive)
        {
            //Time.timeScale = 0.1f;

            animator.SetTrigger(OnHitHash);

            float final = Mathf.Max(0, damage - DefencePower);  // 0 이하로는 데미지가 내려가지 않는다.
            HP -= final;

            onHit?.Invoke(Mathf.RoundToInt(final));
        }
    }

    private void Die()
    {
        onDie?.Invoke();
        animator.SetTrigger(OnDieHash);
        DisablePlayerInput(); // 입력 비활성화

        float resurrectionChance = 0.5f; // 50% 확률

        if (UnityEngine.Random.value < resurrectionChance)
        {
            Resurrection(); // 부활
        }
        else
        {
            Debug.Log("부활 실패");
            //onLoding?.Invoke();
        }
    }

    // 플레이어 입력 비활성화 메서드
    private void DisablePlayerInput()
    {
        inputActions.Player.Disable();
    }

    // 플레이어 입력 활성화 메서드
    private void EnablePlayerInput()
    {
        inputActions.Player.Enable();
    }

    private bool isResurrection = false;

    // 부활 시 입력을 다시 활성화
    private void Resurrection()
    {
        animator.SetTrigger(OnResurrectionHash); // 부활 애니메이션 트리거
        StopAllCoroutines();
        StartCoroutine(ResurrectionEnd());
    }

    private IEnumerator ResurrectionEnd()
    {
        yield return new WaitForSeconds(1.0f);
        hp = MaxHP;
        EnablePlayerInput();
        isResurrection = true;
    }

    void IHealth.Die()
    {
        throw new NotImplementedException();
    }

    public void HealthRegenerate(float totalRegen, float duration)
    {
        throw new NotImplementedException();
    }

    public void HealthRegenerateByTick(float tickRegen, float tickInterval, uint totalTickCount)
    {
        throw new NotImplementedException();
    }
    #endregion
}