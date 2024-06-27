using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_02 : EnemyBase
{
    private Transform BulletSpowner1;
    private Transform BulletSpowner2;

    protected override void Awake()
    {
        base.Awake();

        BulletSpowner1 = transform.GetChild(2);
        BulletSpowner2 = transform.GetChild(3);
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
        Factory.Instance.GetSpownEnemy02_Bullet(BulletSpowner1.position, forward, attackPower);
        Factory.Instance.GetSpownEnemy02_Bullet(BulletSpowner2.position, forward, attackPower);
    }

    protected override void DefenceAddForce()
    {
        rigid.AddForce(Vector2.zero);
        rigid.AddForce(new Vector2(-forward * 1, 1), ForceMode2D.Impulse);
        StartCoroutine(AddForceReset(0.1f));
    }

    private IEnumerator AddForceReset(float time)
    {
        yield return new WaitForSeconds(time);
        rigid.velocity = Vector2.zero;
    }
}
