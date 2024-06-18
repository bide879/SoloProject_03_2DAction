using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDanger : MonoBehaviour
{
    private Animator animator;

    private float dangerHP;
    readonly int IsDangerHash = Animator.StringToHash("IsDanger");

    private Player player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameManager.Instance.Player;
    }

    private void Update()
    {
        if (player != null)
        {
            dangerHP = player.MaxHP / 5 + 1;
            if (player.HP < dangerHP)
            {
                animator.SetBool(IsDangerHash, true);
            }
            else
            {
                animator.SetBool(IsDangerHash, false);
            }
        }
    }
}
