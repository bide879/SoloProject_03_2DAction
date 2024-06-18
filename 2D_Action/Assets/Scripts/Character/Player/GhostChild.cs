using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostChild : Ghost
{
    protected override void OnEnable()
    {
        base.OnEnable();

        if (GameManager.Instance == null)
        {
            //Debug.LogError("GameManager.Instance is null");
            return;
        }

        if (GameManager.Instance.Player == null)
        {
            //Debug.LogError("GameManager.Instance.Player is null");
            return;
        }

        Sprite currentSprite = GameManager.Instance.Player.GetComponentInChildren<SpriteRenderer>().sprite;
        transform.localScale = new Vector3(GameManager.Instance.Player.transform.localScale.x, 1, 1);
        GetComponentInChildren<SpriteRenderer>().sprite = currentSprite;
        StartCoroutine(LifeOver(0.6f));
    }
}
