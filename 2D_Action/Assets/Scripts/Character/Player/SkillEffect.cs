using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkillEffect : RecycleObject
{
    private EnemyBase enemy;
    private Mark mark;

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
        transform.localScale = new Vector3(GameManager.Instance.Player.transform.localScale.x * 2, 2, 1);
        StartCoroutine(LifeOver(0.5f));
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        transform.localScale = new Vector3(2, 2, 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            IBattler target = other.GetComponent<IBattler>();
            if (target != null)
            {
                enemy = other.GetComponent<EnemyBase>();
                GameManager.Instance.Player.SkillAttack(target, enemy.markCount);
                if (enemy.markCount > 0)
                {
                    enemy.markCount = 0;
                }
            }
        }
        else if (other.tag == "Mark")
        {
            mark = other.GetComponentInChildren<Mark>();
            if (mark != null)
            {
                mark.OnTargetDie();
            }
        }
    }

}