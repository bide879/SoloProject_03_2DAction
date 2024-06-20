using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRange : MonoBehaviour
{
    private EnemyBase enemy;
    private Mark mark;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            IBattler target = other.GetComponent<IBattler>();
            if (target != null)
            {
                enemy = other.GetComponent<EnemyBase>();
                GameManager.Instance.Player.Attack(target);
                if(enemy.markCount == 0)
                {
                    Factory.Instance.GetSpownMark(enemy.gameObject);
                }
            }
        }
        else if (other.tag == "Mark")
        {
            mark = other.GetComponentInChildren<Mark>();
            enemy.markCount += 1;
            if (mark != null && enemy.markCount > 1)
            {
                mark.HitMark();
            }
        }
    }
}
