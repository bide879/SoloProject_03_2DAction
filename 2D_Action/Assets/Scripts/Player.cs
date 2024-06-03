using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{

    /// <summary>
    /// 이동속도
    /// </summary>
    private float moveSpeed = 1.0f;

    public float MoveSpeed => moveSpeed;

    /// <summary>
    /// 현재 입력된 이동 방향
    /// </summary>
    Vector2 inputDirection = Vector2.zero;

    // 인풋액션
    PlayerInputActions inputActions;

    Rigidbody2D rigid2D;

    private void Awake()
    {
        inputActions = new PlayerInputActions();        // 인풋 액션 생성
        rigid2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStop;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Jump.canceled += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.canceled -= OnJump;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Move.canceled -= OnStop;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }
    private void FixedUpdate()
    {
        rigid2D.MovePosition(rigid2D.position + (Vector2)(Time.fixedDeltaTime * moveSpeed * inputDirection));
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        //inputDirection.x = context.ReadValue<Vector2>();
    }

    private void OnStop(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        inputDirection = Vector2.zero;
    }

    private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {


    }
}
