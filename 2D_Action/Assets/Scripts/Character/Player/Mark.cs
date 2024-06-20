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
    readonly int OnTargetDieUpHash = Animator.StringToHash("OnTargetDie");
    private float lifeTime;
    private float minLifeTime = 5.0f;

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
            //enemy = target.GetComponent<EnemyBase>();
            enemy.markCount = 0;
            lifeTime = minLifeTime;
            target = null;
        }
       
        base.OnDisable();
    }

    private void Start()
    {
        if (target != null)
        {
            enemy = target.GetComponent<EnemyBase>();
            enemy.onDie += OnTargetDie;
        }
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
        lifeOverCoroutine = StartCoroutine(MarkLifeOver(lifeTime));
    }

    private void StartLifeOverCoroutine(float delay)
    {
        if (lifeOverCoroutine != null)
        {
            StopCoroutine(lifeOverCoroutine);
        }
        lifeOverCoroutine = StartCoroutine(MarkLifeOver(delay));
    }

    public void OnTargetDie()
    {
        if(target != null)
        {
            StopAllCoroutines();
            StartCoroutine(MarkDie());
        }
    }

    /// <summary>
    /// 일정 시간 후에 이 게임 오브젝트를 비활성화 시키는 코루틴
    /// </summary>
    /// <param name="delay">비활성화 될 때까지 걸리는 시간</param>
    /// <returns></returns>
    protected IEnumerator MarkLifeOver(float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay);// delay만큼 기다리고
        animator.SetTrigger(OnTargetDieUpHash);
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);// 비활성화 
    }

    private IEnumerator MarkDie()
    {
        animator.SetTrigger(OnTargetDieUpHash);
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }
   
}
