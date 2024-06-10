using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 이동속도
    /// </summary>
    private float moveSpeed = 5.0f;
    public float MoveSpeed => moveSpeed;

    /// <summary>
    /// 점프 높이
    /// </summary>
    private float jumpPower = 10.0f;
    public float JumpPower => jumpPower;

    private int jumpCount = 0;

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

    // 인풋액션
    PlayerInputActions inputActions;

    Rigidbody2D rigid;

    private bool isGrounded = true;

    Animator animator;
    Ghost ghost;

    public bool isAttackPush = false;
    public bool IsAttackPush => isAttackPush;

    /// <summary>
    /// 애니메이터용 해시값
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int IsJumpHash = Animator.StringToHash("IsJump");

    readonly int OnAttackHash = Animator.StringToHash("OnAttack");
    readonly int IsAttackPushHash = Animator.StringToHash("IsAttackPush");
    readonly int OnDownAttackHash = Animator.StringToHash("OnDownAttack");

    readonly int OnSkillHash = Animator.StringToHash("OnSkill");

    private void Awake()
    {
        inputActions = new PlayerInputActions(); // 인풋 액션 생성
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        normalSpeed = moveSpeed;
        ghost = GetComponentInChildren<Ghost>();
    }

    private void Start()
    {
        ghost.makeGhost = false;
    }


    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStop;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Dash.performed += OnDash;
        inputActions.Player.Dash.canceled += OnDash;
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Skill.performed += OnSkill;
        //inputActions.Player.Attack.canceled += OnAttack;
    }

    private void OnDisable()
    {
        //inputActions.Player.Attack.canceled -= OnAttack;
        inputActions.Player.Skill.performed -= OnSkill;
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Dash.canceled -= OnDash;
        inputActions.Player.Dash.performed -= OnDash;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Move.canceled -= OnStop;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }

    void FixedUpdate()
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
                        isAirDownAttack = false;
                    }
                    isGrounded = true;
                    canJumpAttack = true;
                    jumpCount = 0;
                }
            }
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            inputDirection = Vector2.zero;
        }
        inputDirection = context.ReadValue<Vector2>();
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
    }

    private void OnStop(InputAction.CallbackContext _)
    {
        inputDirection = Vector2.zero;
        animator.SetBool(IsMoveHash, false);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    bool canJumpAttack = true;

    private void Jump()
    {

        if (!isGrounded && jumpCount < 2)
        {
            if (rigid.velocity.y < 0.0)
            {
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
    }

    private void OnDash(InputAction.CallbackContext context)
    {

        if (!context.canceled)
        {
            if (isGrounded)
            {           
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


    // ---------------------------------------- 공격 -----------------------------------------

    private bool canAttack = true;
    //private float attackDelay = 0.23f;
    private float attackDelay = 0.16f;
    private Vector2 previousInputDirection;
    private float previousMoveSpeed;
    private bool isAirDownAttack = false;

    private void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log($"x : {inputDirection.x}, y : {inputDirection.y}");
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
        if (inputDirection.y < 0 && !isGrounded)
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

    private void OnSkill(InputAction.CallbackContext context)
    {
        animator.SetTrigger(OnSkillHash);
        transform.position = new Vector2(transform.position.x + inputDirection.x * 5, transform.position.y + inputDirection.y * 3);
    }
}