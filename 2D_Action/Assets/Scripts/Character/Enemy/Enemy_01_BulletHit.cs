using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_01_BulletHit : RecycleObject
{
    Animator animator;
    float animLength = 0.0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
    }

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
        StartCoroutine(LifeOver(0.5f));
    }

}
