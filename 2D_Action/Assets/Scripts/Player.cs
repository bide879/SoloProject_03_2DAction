using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    private float jumpPower = 5.0f;
    public float JumpPower => jumpPower;

    /// <summary>
    /// 현재 입력된 이동 방향
    /// </summary>
    private Vector2 inputDirection = Vector2.zero;

    // 인풋액션
    PlayerInputActions inputActions;

    Rigidbody2D rigid2D;

    // 지면 체크 관련 변수
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    private Transform groundCheck;
    private float groundCheckRadius = 0.2f;

    private void Awake()
    {
        inputActions = new PlayerInputActions(); // 인풋 액션 생성
        rigid2D = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("GroundCheck");

        // GroundCheck 오브젝트가 없는 경우 생성
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.parent = transform;
            groundCheckObj.transform.localPosition = new Vector3(0, -1, 0); // 발 위치에 맞게 조정
            groundCheck = groundCheckObj.transform;
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStop;
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Move.canceled -= OnStop;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        rigid2D.MovePosition(rigid2D.position + (Vector2)(Time.fixedDeltaTime * moveSpeed * inputDirection));
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        inputDirection.x = context.ReadValue<Vector2>().x;
    }

    private void OnStop(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        inputDirection.x = 0.0f;
    }

    private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        if (isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rigid2D.AddForce(jumpPower * Vector2.up, ForceMode2D.Impulse); // 위쪽으로 jumpPower만큼 힘을 더하기
    }
}