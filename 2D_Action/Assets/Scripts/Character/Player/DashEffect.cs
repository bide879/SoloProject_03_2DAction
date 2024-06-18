using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffect : RecycleObject
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

        transform.localScale = new Vector3(GameManager.Instance.Player.transform.localScale.x, 1, 1);
        StartCoroutine(LifeOver(0.2f));
    }
}
