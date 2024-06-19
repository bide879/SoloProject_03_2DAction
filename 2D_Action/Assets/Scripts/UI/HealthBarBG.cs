using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBG : BarBase
{
    Player player;
    private Coroutine changeCoroutine;

    protected override void OnValueChange(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        if (changeCoroutine != null)
        {
            StopCoroutine(changeCoroutine);
        }
        changeCoroutine = StartCoroutine(SmoothChange(slider.value, ratio, 0.5f));
    }

    private IEnumerator SmoothChange(float startValue, float endValue, float duration)
    {
        yield return new WaitForSeconds(2.0f);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, endValue, elapsed / duration);
            yield return null;
        }
        slider.value = endValue;
    }

    private void Start()
    {
        player = GameManager.Instance.Player;
        if (player != null)
        {
            maxValue = player.MaxHP;
            slider.value = player.HP / maxValue;
            player.onHealthChange += OnValueChange;
        }
    }
}