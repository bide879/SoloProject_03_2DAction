using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_01_Bullet : RecycleObject
{
    /// <summary>
    /// 총알의 이동 속도
    /// </summary>
    private float moveSpeed = 7.0f;

    /// <summary>
    /// 총알의 수명
    /// </summary>
    private float lifeTime = 10.0f;


    protected override void OnEnable()
    {
        base.OnEnable();

        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.Player == null)
        {
            return;
        }
        StartCoroutine(LifeOver(lifeTime));
    }

    private void Update()
    {
        if (GameManager.Instance.Player == null)
        {
            return;
        }
        transform.Translate(Time.deltaTime * moveSpeed * Vector2.right);
    }

    /// <summary>
    /// 부딛혔을 때 실행할 함수
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Factory.Instance.GetSpownEnemy01_BulletHit(transform.position);
            gameObject.SetActive(false);
        }
        if (collision.tag == "AttackRange")
        {
            Factory.Instance.GetSpownEnemy01_BulletHit(transform.position);
            gameObject.SetActive(false);
        }
    }

}
