using System.Collections;
using UnityEngine;
using TMPro;

public class UIHealth : MonoBehaviour
{

    [SerializeField]
    TMP_Text text;
    [SerializeField]
    Color normalColor, damagedColor, healedColor;
    float currentValue = 100f;

    public void UpdateHealthDamaged(float newValue) {
        this.StopAllCoroutines();
        damageAnimate = DamageAnimate( newValue );
        this.StartCoroutine(damageAnimate);
    }
    float length = 0.25f;
    IEnumerator damageAnimate;
    IEnumerator DamageAnimate(float newValue) {
        float timer = 0f;
        float start = currentValue;
        currentValue = newValue;

        while(timer < length) {
            float t = timer / length;
            text.text = Mathf.RoundToInt(Mathf.Lerp(start, newValue, t)).ToString("D3");
            text.rectTransform.localScale = Vector3.Lerp(Vector3.one * 1.3f, Vector3.one, t);
            text.color = Color.Lerp(damagedColor, normalColor, t);
            timer += Time.deltaTime;
            yield return null;
        }
        text.text = Mathf.RoundToInt(newValue).ToString("D3");
        text.rectTransform.localScale = Vector3.one;
        text.color = normalColor;
    }
    
    public void UpdateHealthHealed(float newValue) {
        this.StopAllCoroutines();
        healAnimate = HealAnimate( newValue );
        this.StartCoroutine(healAnimate);
    }
    IEnumerator healAnimate;
    IEnumerator HealAnimate(float newValue) {
        float timer = 0f;
        float start = currentValue;
        currentValue = newValue;

        while(timer < length) {
            float t = timer / length;
            text.text = Mathf.RoundToInt(Mathf.Lerp(start, newValue, t)).ToString("D3");
            text.rectTransform.localScale = Vector3.Lerp(Vector3.one * 1.3f, Vector3.one, t);
            text.color = Color.Lerp(healedColor, normalColor, t);
            timer += Time.deltaTime;
            yield return null;
        }
        text.text = Mathf.RoundToInt(newValue).ToString("D3");
        text.rectTransform.localScale = Vector3.one;
        text.color = normalColor;
    }
}
