using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBackGround : MonoBehaviour
{
    private float cameraSpeed = 4.0f;

    private Player player;
    private GameObject playerObj;

    private void Start()
    {
        player = GameManager.Instance.Player;
        playerObj = player.gameObject;
    }

    private void Update()
    {
        Vector3 dir = playerObj.transform.position - this.transform.position;
        Vector3 moveVector = new Vector3(dir.x * cameraSpeed * Time.deltaTime, (dir.y + 2) * cameraSpeed * Time.deltaTime, 0.0f);
        this.transform.Translate(moveVector);
    }
}
