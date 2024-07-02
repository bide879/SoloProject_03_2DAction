using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : BarBase
{
    private EnemyBase boss;

    public EnemyBase Boss => boss;

    private void Start()
    {
        Boss_01 boss01 = FindAnyObjectByType<Boss_01>();

        boss = boss01;

        if (boss != null)
        {
            maxValue = boss.MaxHP;
            slider.value = boss.HP / maxValue;
            boss.onHealthChange += OnValueChange;
        }
    }


}
