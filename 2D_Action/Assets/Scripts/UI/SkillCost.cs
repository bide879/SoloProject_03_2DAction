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

    private int lastCost = 4;

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
        GameManager.Instance.Player.SkillCostChang += OnChangCostImage;
    }

    private void OnChangCostImage(int slot)
    {
        if(slot > 4)
        {
            StartCoroutine(ImageColorChang());
        }
        else
        {
            if (costAnimators[slot] != null)
            {
                costAnimators[slot].SetTrigger(OnConsumptionHash);
                lastCost = slot;
            }
        }
    }

    IEnumerator ImageColorChang()
    {
        for (int i = lastCost; i < costAnimators.Length; i++)
        {
            costAnimators[i].SetTrigger(OnCostChargeHash);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
