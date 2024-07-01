using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHitEffect : RecycleObject
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

        StartCoroutine(LifeOver(0.5f));
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }

}
