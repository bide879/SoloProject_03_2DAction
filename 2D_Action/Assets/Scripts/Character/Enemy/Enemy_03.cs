using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_03 : EnemyBase
{
    Vector2 attackStartVector;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void EnemyRay()
    {
        Debug.DrawRay(rigid.position, Vector3.right * -5.0f * forward, new Color(0, 1, 0));
        rayBackHit = Physics2D.Raycast(rigid.position, Vector3.right * forward, -5.0f, LayerMask.GetMask("Player"));

        Debug.DrawRay(rigid.position, Vector3.right * 5.0f * forward, new Color(1, 0, 0));
        rayAttackHit = Physics2D.Raycast(rigid.position, Vector3.right * forward, 5.0f, LayerMask.GetMask("Player"));
    }

    protected override void DefenceAddForce()
    {
        rigid.AddForce(Vector2.zero);
        rigid.AddForce(new Vector2(-forward * 1, 1), ForceMode2D.Impulse);
    }

    public void AttackStart()
    {
        attackStartVector = transform.position;
    }

    public void Attack1()
    {
        rigid.AddForce(Vector2.zero);
        rigid.AddForce(new Vector2(forward * 4, 0), ForceMode2D.Impulse);
    }

    public void Attack2()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + 1.0f);
    }

    public void AttackEnd()
    {
        transform.position = attackStartVector;
    }
}
