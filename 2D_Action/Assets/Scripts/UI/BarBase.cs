using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarBase : MonoBehaviour
{
    protected Slider slider;

    protected float maxValue;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    protected virtual void OnValueChange(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        slider.value = ratio;
    }

}
