using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mark : RecycleObject
{
    public GameObject target;
    private Animator animator;
    private EnemyBase enemy;
    readonly int OnMarkLvUpHash = Animator.StringToHash("OnMarkLvUp");
    private float lifeTime;
    private float minLifeTime = 20.0f;

    private Coroutine lifeOverCoroutine; // 현재 진행 중인 LifeOver 코루틴을 저장하는 변수

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

        animator = GetComponent<Animator>();

        lifeTime = minLifeTime;
        StartLifeOverCoroutine(lifeTime); // LifeOver 코루틴 시작
    }

    protected override void OnDisable()
    {
        if(target != null)
        {
            enemy = target.GetComponent<EnemyBase>();
            enemy.markCount = 0;
            lifeTime = minLifeTime;
            target = null;
        }
       
        base.OnDisable();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance == null)
        {
            return;
        }
        if (GameManager.Instance.Player == null)
        {
            return;
        }

        if(target != null)
        {
            transform.position = target.transform.position;
            if(enemy != null)
            {

            }
        }
    }

    public void HitMark() 
    {
        animator.SetTrigger(OnMarkLvUpHash);

        // 현재 진행 중인 LifeOver 코루틴 중지
        if (lifeOverCoroutine != null)
        {
            StopCoroutine(lifeOverCoroutine);
        }
        lifeOverCoroutine = StartCoroutine(LifeOver(lifeTime));
    }

    private void StartLifeOverCoroutine(float delay)
    {
        if (lifeOverCoroutine != null)
        {
            StopCoroutine(lifeOverCoroutine);
        }
        lifeOverCoroutine = StartCoroutine(LifeOver(delay));
    }
}
