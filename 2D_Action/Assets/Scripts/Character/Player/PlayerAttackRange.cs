using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
        {
            IBattler target = other.GetComponent<IBattler>();
            if (target != null)
            {
                GameManager.Instance.Player.Attack(target);
                Debug.Log("적에게 명중");
            }
        }
    }
}
