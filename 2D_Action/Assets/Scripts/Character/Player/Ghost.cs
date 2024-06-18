using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : RecycleObject
{
    public float ghostDelay;
    private float ghostDelayTime;
    public GameObject ghost;
    public bool makeGhost = false;
    public bool playerOnAttack;

    void Start()
    {
        this.ghostDelayTime = this.ghostDelay;
    }

    void FixedUpdate()
    {
        onAttack(playerOnAttack);
        //transform.localScale
        if (this.makeGhost)
        {
            if (this.ghostDelayTime > 0)
            {
                this.ghostDelayTime -= Time.deltaTime;
            }
            else
            {
                Factory.Instance.GetSpownGhost(this.ghost, this.transform.position, this.transform.rotation);
                this.ghostDelayTime = this.ghostDelay;
            }
            
        }
    }

    public void onAttack(bool attack)
    {
        GameManager.Instance.Player.isAttackPush = attack;
    }
}
