using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack01Bullet : RecycleObject
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
    }

    private void Update()
    {
        if (GameManager.Instance.Player == null)
        {
            return;
        }
        transform.Translate(Time.deltaTime * moveSpeed * Vector2.right * forward);
    }
}
