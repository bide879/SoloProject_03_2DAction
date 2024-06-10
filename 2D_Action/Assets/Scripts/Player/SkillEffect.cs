using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkillEffect : RecycleObject
{
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

        transform.localScale = new Vector3(GameManager.Instance.Player.transform.localScale.x * this.transform.localScale.x, this.transform.localScale.y, 1);
        StartCoroutine(LifeOver(0.5f));
    }
}
