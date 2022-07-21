using System.Collections;
using UnityEngine;
using TMPro;

public class UIHud : MonoBehaviour
{
    
    [SerializeField]
    UIBar reticleHealthBar, ammoBar;
    [SerializeField]
    UIHealth health;

    [SerializeField]
    TMP_Text ammoCounter, pistolAmmoCounter, shotgunAmmoCounter, rifleAmmoCounter, rocketAmmoCounter, weaponName;

    [SerializeField]
    UIWeaponList weapons;
    public UIWeaponList Weapons => weapons;

    [SerializeField]
    GameObject pickUpText;
    float barAnimationLength = 0.1f;

    public void SetPickUpTextEnabled(bool enabled) {
        pickUpText.SetActive(enabled);
    }

    public void UpdateHealth( float newHealth, float maxHealth ) {
        health.UpdateHealthDamaged(newHealth);
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
        float startValue = reticleHealthBar.Value;
        while( timer < barAnimationLength ) {
            float t = timer / barAnimationLength;
            float value = Mathf.Lerp(startValue, newValue, t);
            reticleHealthBar.SetValue(value);
            timer += Time.deltaTime;
            yield return null;
        }
        reticleHealthBar.SetValue(newValue);
    }

    public void UpdateAmmoCounter( int count, int max ) {
        ammoCounter.gameObject.SetActive(true);
        ammoCounter.text = count.ToString("D3");
        ammoBar.SetValue((float)count / (float)max);
    }
    public void UpdateReserveCounters( int pistol, int shotgun, int rifle, int rocket ) {
        pistolAmmoCounter.text = pistol.ToString("D3");
        shotgunAmmoCounter.text = shotgun.ToString("D3");
        rifleAmmoCounter.text = rifle.ToString("D3");
        rocketAmmoCounter.text = rocket.ToString("D3");
    }

    public void NoWeapon() {
        ammoCounter.gameObject.SetActive(false);
        weaponName.text = "";
    }

    public void UpdateWeaponName(string text) {
        weaponName.text = text;
    }
}
