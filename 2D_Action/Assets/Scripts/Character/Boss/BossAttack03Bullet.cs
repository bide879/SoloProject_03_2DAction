using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack03Bullet : RecycleObject
{
    /// <summary>
    /// 총알의 수명
    /// </summary>
    private float lifeTime = 0.35f;

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
}
