using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestEnemy : TestBase
{
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        GameManager.Instance.Player.HP -= 10;
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        GameManager.Instance.Player.HP += 10;
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        //Factory.Instance.GetSpownEnemy01_Bullet(transform.position, 1, 1);
    }
}
