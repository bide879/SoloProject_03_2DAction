using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_02_Bullet : RecycleObject
{
    /// <summary>
    /// 총알의 이동 속도
    /// </summary>
    private float moveSpeed = 7.0f;

    /// <summary>
    /// 총알의 수명
    /// </summary>
    private float lifeTime = 5.0f;

    public float forward = 1;
    private int hpCount = 0;

    public float damage;

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
        hpCount = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.Player == null)
        {
            return;
        }
        transform.Translate(Time.deltaTime * moveSpeed * Vector2.right * forward);
    }

    /// <summary>
    /// 부딛혔을 때 실행할 함수
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Factory.Instance.GetSpownEnemy02_BulletHit(transform.position);
            gameObject.SetActive(false);
        }
        if (collision.tag == "AttackRange")
        {
            if (hpCount > 0)
            {
                Factory.Instance.GetSpownEnemy02_BulletHit(transform.position);
                gameObject.SetActive(false);
            }
            hpCount++;
        }
    }
}
