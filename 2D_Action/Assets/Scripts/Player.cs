using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 이동속도
    /// </summary>
    private float moveSpeed = 4.0f;
    public float MoveSpeed => moveSpeed;

    /// <summary>
    /// 점프 높이
    /// </summary>
    private float jumpPower = 15.0f;
    public float JumpPower => jumpPower;

    /// <summary>
    /// 이동속도
    /// </summary>
    private float normalSpeed;

    private float dashSpeed = 6.0f;
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

    /// <summary>
    /// 애니메이터용 해시값
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int IsJumpHash = Animator.StringToHash("IsJump");

    private void Awake()
    {
        inputActions = new PlayerInputActions(); // 인풋 액션 생성
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        normalSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStop;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Dash.performed += OnDash;
        inputActions.Player.Dash.canceled += OnDash;
    }

    private void OnDisable()
    {
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
                Debug.Log("바닥 확인");
                if (rayHit.distance < 1f)
                {
                    Debug.Log("바닥에 닿음");
                    animator.SetBool(IsJumpHash, false);
                    isGrounded = true;
                }
            }
        }
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        inputDirection.x = context.ReadValue<Vector2>().x;
        animator.SetBool(IsMoveHash, true);
        if (0 > inputDirection.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void OnStop(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        inputDirection.x = 0.0f;
        animator.SetBool(IsMoveHash, false);
    }

    private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        Jump();
    }

    private void Jump()
    {
        Debug.Log("점프 누름");
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        animator.SetBool(IsJumpHash, true);
        isGrounded = false;
    }

    private void OnDash(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            if(isGrounded)
            {
                Debug.Log("대시 누름");
                moveSpeed = dashSpeed;
            }
        }
        else
        {
            if (isGrounded)
            {
                Debug.Log("대시 땜");
                moveSpeed = normalSpeed;
            }
        }
    }
}