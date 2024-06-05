using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelayTime;
    public GameObject ghost;
    public bool makeGhost;
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
                /*
                GameObject currentGhost = Factory.Instance.GetSpownGhost(this.ghost, this.transform.position, this.transform.rotation).gameObject;
                Sprite currentSprite = this.GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.localScale = new Vector3(
                    GameManager.Instance.Player.transform.localScale.x * this.transform.localScale.x,
                    this.transform.localScale.y,
                    this.transform.localScale.z
                );
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                this.ghostDelayTime = this.ghostDelay;
                */             
                
                GameObject currentGhost = Instantiate(this.ghost, this.transform.position, this.transform.rotation);
                Sprite currentSprite = this.GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.localScale = new Vector3(GameManager.Instance.Player.transform.localScale.x * this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                this.ghostDelayTime = this.ghostDelay;
                Destroy(currentGhost, 0.6f);
                
            }
            
        }
    }

    public void onAttack(bool attack)
    {
        GameManager.Instance.Player.isAttackPush = attack;
    }
}
