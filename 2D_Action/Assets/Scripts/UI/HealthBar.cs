using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : BarBase
{
    private void Start()
    {
        Player player = GameManager.Instance.Player;
        if (player != null)
        {
            maxValue = player.MaxHP;
            slider.value = player.HP / maxValue;
            player.onHealthChange += OnValueChange;
        }
    }
}