using System.Collections;
using UnityEngine;

public class UIHud : MonoBehaviour
{
    
    [SerializeField]
    UIBar reticleHealthBar, healthBar, ammoBar;
    [SerializeField]
    UIAmmoCounter ammoCounter;
    [SerializeField]
    UIAmmoType ammoType;
    [SerializeField]
    TMPro.TMP_Text qualityText;
    float barAnimationLength = 0.1f;

    public void UpdateHealth( float newHealth, float maxHealth ) {
        float value = newHealth / maxHealth;
        if( animatedHealthBar != null ) {
            this.StopCoroutine(animatedHealthBar);
        }
        animatedHealthBar = AnimatedHealthBar(value);
        this.StartCoroutine(animatedHealthBar);
    }
    IEnumerator animatedHealthBar;
    IEnumerator AnimatedHealthBar( float newValue ) {
        float timer = 0f;
        float startValue = healthBar.Value;
        while( timer < barAnimationLength ) {
            float t = timer / barAnimationLength;
            float value = Mathf.Lerp(startValue, newValue, t);
            reticleHealthBar.SetValue(value);
            healthBar.SetValue(value);
            timer += Time.deltaTime;
            yield return null;
        }
        reticleHealthBar.SetValue(newValue);
        healthBar.SetValue(newValue);
    }

    public void UpdateAmmo( int count, int totalMagazine, int totalReserve ) {
        ammoCounter.gameObject.SetActive(true);
        ammoType.gameObject.SetActive(true);
        ammoCounter.UpdateCounter(count, totalReserve);
        ammoBar.SetValue( (float)count / (float)totalMagazine );
    }

    public void UpdateAmmoType( GunType type ) {
        ammoType.SetAmmoType(type);
    }

    public void DisableAmmo() {
        ammoCounter.gameObject.SetActive(false);
        ammoType.gameObject.SetActive(false);
    }

    public void UpdateWeaponQuality(string text) {
        qualityText.text = text;
    }
}
