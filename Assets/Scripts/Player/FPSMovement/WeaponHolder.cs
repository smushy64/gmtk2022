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

    private List<HitscanWeapon> weaponList = new List<HitscanWeapon>();
    private int currentWeapon = 0;

    private UIHud hud;
    private Ammo ammo;
    private PlayerInput input;
    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction changeWeapon;

    private bool isFiring = false;
    private bool isReloading = false;
    private bool weaponEquipped = false;

    private void Awake()
    {
        hud = FindObjectOfType<UIHud>();
        input = GetComponentInParent<PlayerInput>();
        ammo = GetComponent<Ammo>();
        fireAction = input.actions["Fire"];
        reloadAction = input.actions["Reload"];
        changeWeapon = input.actions["Change Weapon"];
        fireAction.performed += OnStartFire;
        fireAction.canceled += OnStopFire;
        reloadAction.performed += OnReload;
        changeWeapon.performed += OnWeaponChanged;
    }

    private void Start()
    {
        weapon.Equip(() => weaponEquipped = true);
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

    private void OnWeaponChanged(InputAction.CallbackContext ctx)
    {
        float value = changeWeapon.ReadValue<Vector2>().y;
        if (value == 0)
            return;

        int sign = (int)Mathf.Sign(value);
        currentWeapon += sign;

        if (currentWeapon < 0)
            currentWeapon = weaponList.Count - 1;
        if (currentWeapon >= weaponList.Count)
            currentWeapon = 0;

        if (weaponEquipped)
        {
            // cancel reload
            isReloading = false;
            weaponEquipped = false;
            weapon.Unequip();
        }
        else
        {
            weapon.Equip(() => weaponEquipped = true);
        }
    }

    private void Update()
    {
        if (weaponEquipped && !isReloading && isFiring && weapon.CanFire)
        {
            if (weapon.AmmoLeft > 0)
            {
                weapon.Fire(cam.transform.position, cam.transform.forward);
            }
        }
        
        // Automatically start reload if we're out of bullets
        if (weaponEquipped && !isReloading && weapon.CanReload && weapon.AmmoLeft <= 0 && ammo.GetReserve(weapon.Type) > 0)
        {
            ReloadSelectedWeapon();
        }

        hud.UpdateAmmo(weapon.AmmoLeft, weapon.MaxAmmo, ammo.GetReserve(weapon.Type));
    }

    private void ReloadSelectedWeapon()
    {
        isReloading = true;
        weapon.StartReload(() =>
        {
            int ammoToConsume = weapon.MaxAmmo - weapon.AmmoLeft;
            weapon.AddAmmo(ammo.Consume(weapon.Type, ammoToConsume));
            isReloading = false;
        });
    }

    private void ClearWeapons()
    {
        currentWeapon = 0;
        for (int i = weaponList.Count - 1; i >= 0; i--)
        {
            Destroy(weaponList[i].gameObject);
            weaponList.RemoveAt(i);
        }
    }

    public void AddWeapon(HitscanWeapon weapon)
    {
    }
}
