using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_01 : EnemyBase
{
    private Transform BulletSpowner;

    protected override void Awake()
    {
        base.Awake();
        BulletSpowner = transform.GetChild(2);
    }

    protected override void EnemyRay()
    {
        Debug.DrawRay(rigid.position + Vector2.down * 0.5f, Vector3.right * -5.0f * forward, new Color(0, 1, 0));
        rayBackHit = Physics2D.Raycast(rigid.position + Vector2.down * 0.5f, Vector3.right * forward, -5.0f, LayerMask.GetMask("Player"));

        Debug.DrawRay(rigid.position + Vector2.down * 0.5f, Vector3.right * 7.0f * forward, new Color(1, 0, 0));
        rayAttackHit = Physics2D.Raycast(rigid.position + Vector2.down * 0.5f, Vector3.right * forward, 7.0f, LayerMask.GetMask("Player"));
    }

    protected override void OnFire()
    {
        Factory.Instance.GetSpownEnemy01_Bullet(BulletSpowner.position, forward, attackPower);
    }

    protected override void DefenceAddForce()
    {
        rigid.AddForce(Vector2.zero);
        rigid.AddForce(new Vector2(-forward * 1, 1), ForceMode2D.Impulse);
    }
}
