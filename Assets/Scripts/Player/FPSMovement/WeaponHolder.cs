using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Docien.FPSMovement;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject pivot;
    [SerializeField] private HitscanWeapon weapon;

    private UIHud hud;
    private Ammo ammo;
    private PlayerInput input;
    private InputAction fireAction;
    private InputAction reloadAction;

    private bool isFiring = false;

    private void Awake()
    {
        hud = FindObjectOfType<UIHud>();
        input = GetComponentInParent<PlayerInput>();
        ammo = GetComponent<Ammo>();
        fireAction = input.actions["Fire"];
        reloadAction = input.actions["Reload"];
        fireAction.performed += OnStartFire;
        fireAction.canceled += OnStopFire;
        reloadAction.performed += OnReload;
    }

    private void OnStartFire(InputAction.CallbackContext ctx) => isFiring = true;
    private void OnStopFire(InputAction.CallbackContext ctx) => isFiring = false;
    private void OnReload(InputAction.CallbackContext ctx)
    {
        if (weapon.CanReload && ammo.GetReserve(weapon.Type) > 0)
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
        if (weapon.CanReload && weapon.AmmoLeft <= 0 && ammo.GetReserve(weapon.Type) > 0)
        {
            ReloadSelectedWeapon();
        }

        hud.UpdateAmmo(weapon.AmmoLeft, weapon.MaxAmmo, ammo.GetReserve(weapon.Type));
    }

    private void ReloadSelectedWeapon()
    {
        weapon.StartReload(() =>
        {
            int ammoToConsume = weapon.MaxAmmo - weapon.AmmoLeft;
            weapon.AddAmmo(ammo.Consume(weapon.Type, ammoToConsume));
        });
    }
}
