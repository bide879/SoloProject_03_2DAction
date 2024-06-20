using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    Player player;
    public Player Player => player;

    EnemyBase enemy;

    public EnemyBase Enemy => enemy;


    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        enemy = FindAnyObjectByType<EnemyBase>();
    }
}
