using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Docien.FPSMovement;

public enum AmmoType
{
    Shotgun,
    Pistol,
    Rifle
}

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject pivot;
    [SerializeField] private HitscanWeapon weapon;
    [SerializeField] private int maxShotgunReserve;
    [SerializeField] private int maxPistolReserve;
    [SerializeField] private int maxRifleReserve;

    private int shotgunAmmoReserve;
    private int pistolAmmoReserve;
    private int rifleAmmoReserve;

    private UIHud hud;
    private PlayerInput input;
    private InputAction fireAction;
    private InputAction reloadAction;

    private bool isFiring = false;

    private void Awake()
    {
        hud = FindObjectOfType<UIHud>();
        input = GetComponentInParent<PlayerInput>();
        fireAction = input.actions["Fire"];
        reloadAction = input.actions["Reload"];
        fireAction.performed += OnStartFire;
        fireAction.canceled += OnStopFire;
        reloadAction.performed += OnReload;

        shotgunAmmoReserve = maxShotgunReserve;
        pistolAmmoReserve = maxPistolReserve;
        rifleAmmoReserve = maxRifleReserve;
    }

    private void OnStartFire(InputAction.CallbackContext ctx) => isFiring = true;
    private void OnStopFire(InputAction.CallbackContext ctx) => isFiring = false;
    private void OnReload(InputAction.CallbackContext ctx)
    {
        if (weapon.CanReload && GetAmmoReserve(weapon.Type) > 0)
        {
            ReloadSelectedWeapon();
        }
    }

    private void Update()
    {
        if (!weapon.IsReloading && isFiring && weapon.CanFire)
        {
            if (weapon.AmmoLeft > 0)
            {
                weapon.Fire(cam.transform.position, cam.transform.forward);
            }
        }
        
        // Automatically start reload if we're out of bullets
        if (weapon.CanReload && weapon.AmmoLeft <= 0 && GetAmmoReserve(weapon.Type) > 0)
        {
            ReloadSelectedWeapon();
        }

        hud.UpdateAmmo(weapon.AmmoLeft, weapon.MaxAmmo, GetAmmoReserve(weapon.Type));
    }

    private void ReloadSelectedWeapon()
    {
        weapon.StartReload(() =>
        {
            int ammoToConsume = weapon.MaxAmmo - weapon.AmmoLeft;
            weapon.AddAmmo(ConsumeAmmo(weapon.Type, ammoToConsume));
        });
    }

    // Passes in the amount of ammo we need, and returns the amount that we can give.
    private int ConsumeAmmo(AmmoType type, int amount)
    {
        switch (type)
        {
            case AmmoType.Shotgun:
                return ConsumeAmmoType(ref shotgunAmmoReserve, amount);
            case AmmoType.Pistol:
                return ConsumeAmmoType(ref pistolAmmoReserve, amount);
            case AmmoType.Rifle:
                return ConsumeAmmoType(ref rifleAmmoReserve, amount);
            default:
                Debug.LogError("The currently selected weapon does not yet have an implemented ammo type.");
                return 0;
        }

        int ConsumeAmmoType(ref int reserve, int ammoToConsume)
        {
            ammoToConsume = Mathf.Min(ammoToConsume, reserve);
            reserve -= ammoToConsume;
            return ammoToConsume;
        }
    }

    private int GetAmmoReserve(AmmoType type) => type switch
    {
        AmmoType.Shotgun => shotgunAmmoReserve,
        AmmoType.Pistol => pistolAmmoReserve,
        AmmoType.Rifle => rifleAmmoReserve,
        _ => throw new System.NotImplementedException()
    };
}
