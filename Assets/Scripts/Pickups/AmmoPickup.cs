using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoPickup : MonoBehaviour
{
    private GunType type;

    [SerializeField] private Sprite pistolAmmoSprite;
    [SerializeField] private Sprite shotgunAmmoSprite;
    [SerializeField] private Sprite rifleAmmoSprite;
    [SerializeField] private Sprite rocketAmmoSprite;
    private Image image;

    private int pistolAmmo = 6;
    private int shotgunAmmo = 2;
    private int rifleAmmo = 4;
    private int rocketAmmo = 1;

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        type = (GunType)Random.Range(0, 4);
        ChangeSprite();
    }

    private void ChangeSprite()
    {
        switch (type)
        {
            case GunType.Pistol:
                image.sprite = pistolAmmoSprite;
                break;
            case GunType.Shotgun:
                image.sprite = shotgunAmmoSprite;
                break;
            case GunType.Rifle:
                image.sprite = rifleAmmoSprite;
                break;
            case GunType.Rocket:
                image.sprite = rocketAmmoSprite;
                break;
            default:
                break;
        }
    }

    private int Ammo()
    {
        switch (type)
        {
            case GunType.Pistol:
                Instantiate(pistolAmmoSprite);
                return pistolAmmo;
            case GunType.Shotgun:
                Instantiate(shotgunAmmoSprite);
                return shotgunAmmo;
            case GunType.Rifle:
                Instantiate(rifleAmmoSprite);
                return rifleAmmo;
            case GunType.Rocket:
                Instantiate(rocketAmmoSprite);
                return rocketAmmo;
        }

        return 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var wm = other.GetComponentInChildren<WeaponManager>();
            if (wm != null)
            {
                Debug.Log("ALDLSDSJKDL");
                wm.ChangeAmmoCount(type, -Ammo());
                Destroy(gameObject);
            }
        }
    }
}
