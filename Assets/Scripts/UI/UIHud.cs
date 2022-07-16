using UnityEngine;

public class UIHud : MonoBehaviour
{
    
    [SerializeField]
    UIBar reticleHealthBar, healthBar, ammoBar;
    [SerializeField]
    UIAmmoCounter ammoCounter;
    [SerializeField]
    UIAmmoType ammoType;

    public void UpdateHealth( float newHealth, float maxHealth ) {
        float value = newHealth / maxHealth;
        reticleHealthBar.SetValue(value);
        healthBar.SetValue(value);
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

}
