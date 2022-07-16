using UnityEngine;

public class UIHud : MonoBehaviour
{
    
    [SerializeField]
    UIBar reticleHealthBar, healthBar, ammoBar;
    [SerializeField]
    UIAmmoCounter ammoCounter;

    public void UpdateHealth( float newHealth, float maxHealth ) {
        float value = newHealth / maxHealth;
        reticleHealthBar.SetValue(value);
        healthBar.SetValue(value);
    }

    public void UpdateAmmo( int count, int totalMagazine, int totalReserve) {
        ammoCounter.UpdateCounter(count, totalReserve);
        ammoBar.SetValue( (float)count / (float)totalMagazine );
    }

}
