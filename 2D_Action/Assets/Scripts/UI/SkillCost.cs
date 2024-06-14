using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCost : MonoBehaviour
{
    Image[] skillCostImg;
    public Color color;

    private void Awake()
    {
        skillCostImg = new Image[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform slot = transform.GetChild(i);
            Transform costChild = slot.GetChild(1);
            skillCostImg[i] = costChild.GetComponent<Image>();
        }
    }

    private void Start()
    {
        GameManager.Instance.Player.SkillCostChang += OnChangCostImage;
    }

    private void OnChangCostImage(int slot)
    {
        if(slot > 4)
        {
            for(int i = 0; i < skillCostImg.Length; i++)
            {
                skillCostImg[i].color = color;
            }     
        }
        else
        {
            if (skillCostImg[slot] != null)
            {
                skillCostImg[slot].color = Color.clear;
            }
        }
    }

    IEnumerator ImageColorChang(bool cost)
    {
        if (!cost)
        {
            color.a += Time.deltaTime;
        }
        else
        {

        }

        yield return null;
    }
}
