using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillCost : MonoBehaviour
{
    Animator[] costAnimators;

    readonly int OnConsumptionHash = Animator.StringToHash("OnConsumption");
    readonly int OnCostChargeHash = Animator.StringToHash("OnCostCharge");

    private int lastCost = 5;

    private void Awake()
    {
        costAnimators = new Animator[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform slot = transform.GetChild(i);
            costAnimators[i] = slot.GetComponent<Animator>();
        }
    }

    private void Start()
    {
        GameManager.Instance.Player.SkillCostChang += OnChangCost;
    }

    private void OnChangCost(int slot)
    {
        if (slot != 5 && slot >= lastCost)
        {
            StartCoroutine(ChangCost(slot));
            lastCost = slot;
        }
        else if(slot > 4)
        {
            StartCoroutine(ChangAllCost());
            lastCost = 5;
        }
        else
        {
            if (costAnimators[slot] != null)
            {
                costAnimators[slot].SetTrigger(OnConsumptionHash);
                if(lastCost != 0)
                {
                    lastCost -= 1;
                }
            }
        }
    }

    IEnumerator ChangAllCost()
    {
        for (int i = lastCost; i < costAnimators.Length; i++)
        {
            costAnimators[i].SetTrigger(OnCostChargeHash);
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator ChangCost(int slot)
    {
        if(slot > 4)
        {
            slot = 5;
        }
        for (int i = lastCost; i < slot; i++)
        {
            costAnimators[i].SetTrigger(OnCostChargeHash);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
