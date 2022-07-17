using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Docien.FPSMovement;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    Hitscan hitscanWeapon;
    [SerializeField]
    Projectile projectileWeapon;

    [SerializeField]
    UIHud hud;

    PlayerInput input;
    PlayerLook look;

    InputAction fire;
    InputAction switchWeapon;
    InputAction reloadWeapon;

    public Action OnWeaponStartSwitch;
    public Action OnWeaponSwitch;
    public Action OnWeaponEndSwitch;
    public Action OnWeaponFire;
    public Action OnWeaponStartReload;
    public Action OnWeaponEndReload;

    int pistolAmmo  = 20;
    int shotgunAmmo = 5;
    int rifleAmmo   = 25;
    int rocketAmmo  = 3;
    int energyAmmo  = 40;

    int currentWeaponIndex = 0;
    const int MAX_WEAPON_COUNT = 9;
    GunData[] weapons = new GunData[MAX_WEAPON_COUNT];
    bool weaponsContainsNull = true;

    public GunData CurrentWeapon => weapons[currentWeaponIndex];

    private void Awake()
    {
        input = GetComponentInParent<PlayerInput>();
        look = GetComponentInParent<PlayerLook>();
        fire = input.actions["Fire"];
        switchWeapon = input.actions["Change Weapon"];
        reloadWeapon = input.actions["Reload"];

        for (int i = 0; i < MAX_WEAPON_COUNT; ++i) {
            weapons[i] = new GunData();
        }

        CheckForEmptyWeaponSlot();

        hud.DisableAmmo();
    }

    public void ChangeAmmoCount( GunType type, int delta ) {
        UpdateReserve(type, delta);
        if( CurrentWeapon != null ) {
            hud.UpdateAmmo(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity, ReserveAmmo());
        }
    }

    public void SwapWeapon( GunData newWeapon ) {
        if( weaponsContainsNull ) {
            for (int i = 0; i < weapons.Length; ++i){
                if ( weapons[i] == null ) {
                    Debug.Log("Weapon slots are not filled, adding new weapon!");
                    weapons[i] = newWeapon;
                    currentWeaponIndex = i;
                    if( i == weapons.Length - 1 ) {
                        weaponsContainsNull = false;
                    }
                }
            }
        } else {
            Debug.Log("Weapon slots are filled, swapping weapon!");
            weapons[currentWeaponIndex] = newWeapon;
            // TODO: spawn swapped weapon as pick up?
        }

        UpdateSelectedWeapon();
    }

    private void CheckForEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i] == null)
            {
                weaponsContainsNull = true;
            }
        }

        weaponsContainsNull = false;
    }

    void OnSwitchWeapon(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() > 0f)
        {
            SwitchWeapon(1);
        }
        else
        {
            SwitchWeapon(-1);
        }

        UpdateSelectedWeapon();
    }

    private void UpdateSelectedWeapon()
    {
        fireRateCanShoot = true;
        rifleIsFiring = false;
        isReloading = false;
        reloadDelay = null;
        shootDelay = null;

        StartWeaponSwitchDelay();

        if (CurrentWeapon != null)
        {
            // TODO: remove this
            Debug.Log("Index: " + currentWeaponIndex + " | " + CurrentWeapon.name);
            hud.UpdateAmmo(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity, ReserveAmmo());
            hud.UpdateAmmoType(CurrentWeapon.type);
            hitscanWeapon.SetSpreadMultiplier(CurrentWeapon.recoilMultiplier);
        }
    }

    public const float WEAPON_SWITCH_DELAY = 1f;
    bool isSwitching = false;
    void StartWeaponSwitchDelay() {
        if( switchWeaponDelay != null ) {
            this.StopCoroutine(switchWeaponDelay);
        }
        switchWeaponDelay = SwitchWeaponDelay(WEAPON_SWITCH_DELAY);
        this.StartCoroutine(switchWeaponDelay);
    }

    IEnumerator switchWeaponDelay;
    IEnumerator SwitchWeaponDelay( float delay ) {
        float timer = 0f;
        OnWeaponStartSwitch?.Invoke();
        bool calledMidSwitch = false;
        isSwitching = true;
        while( timer < delay ) {
            timer += Time.deltaTime;
            if( !calledMidSwitch && timer >= delay / 2f ) {
                calledMidSwitch = true;
                OnWeaponSwitch?.Invoke();
            }
            yield return null;
        }
        isSwitching = false;
        OnWeaponEndSwitch?.Invoke();
    }

    void SwitchWeapon(int delta) {
        currentWeaponIndex = WrapAddWeaponIndex(delta);
        for (int i = 0; i < MAX_WEAPON_COUNT; ++i ) {
            if( CurrentWeapon != null )
                return;
            else
                currentWeaponIndex = WrapAddWeaponIndex(delta);
        }
    }

    int WrapAddWeaponIndex( int delta ) {
        int result = currentWeaponIndex + delta;
        if( result < 0 ) {
            return MAX_WEAPON_COUNT - 1;
        } else if( result >= MAX_WEAPON_COUNT ) {
            return 0;
        } else {
            return result;
        }
    }

    void OnFire( InputAction.CallbackContext ctx ) {
        if( CurrentWeapon == null || isReloading || isSwitching )
            return;

        if( CurrentWeapon.ammoCount == 0 ) {
            Reload();
            return;
        }

        switch( CurrentWeapon.type ) {
            case GunType.Rifle:
                StartRifleFire( CurrentWeapon.delayBetweenShots );
                break;
            case GunType.Energy:
            case GunType.Rocket:
                if (fireRateCanShoot) {
                    if( CurrentWeapon.Shoot() ) {
                        OnWeaponFire?.Invoke();
                        projectileWeapon.Fire();
                        ShootDelay(CurrentWeapon.delayBetweenShots);
                        hud.UpdateAmmo(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity, ReserveAmmo());
                        look.Recoil();
                    }
                }
                break;
            case GunType.Shotgun:
                if (fireRateCanShoot) {
                    if( CurrentWeapon.Shoot() ) {
                        OnWeaponFire?.Invoke();
                        hitscanWeapon.Fire( CurrentWeapon.pelletsPerShot, Mathf.RoundToInt(CurrentWeapon.damagePerShot));
                        ShootDelay(CurrentWeapon.delayBetweenShots);
                        hud.UpdateAmmo(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity, ReserveAmmo());
                        look.Recoil();
                    }
                }
                break;
            default:
                if(fireRateCanShoot) {
                    if (CurrentWeapon.Shoot()) {
                        OnWeaponFire?.Invoke();
                        hitscanWeapon.Fire( Mathf.RoundToInt(CurrentWeapon.damagePerShot) );
                        ShootDelay(CurrentWeapon.delayBetweenShots);
                        hud.UpdateAmmo(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity, ReserveAmmo());
                        look.Recoil();
                    }
                }
                break;
        }
    }

    void OnFireUp( InputAction.CallbackContext ctx ) {
        rifleIsFiring = false;
    }

    void ShootDelay( float delay ) {
        if( shootDelay != null ) {
            this.StopCoroutine(shootDelay);
        }
        shootDelay = ShootDelayRoutine(delay);
        this.StartCoroutine(shootDelay);
    }

    bool fireRateCanShoot = true;
    IEnumerator shootDelay;
    IEnumerator ShootDelayRoutine( float delay ) {
        float timer = 0f;
        fireRateCanShoot = false;
        while( timer < delay ) {
            timer += Time.deltaTime;
            yield return null;
        }
        fireRateCanShoot = true;
    }

    void StartRifleFire( float delayBetweenShots ) {
        if (CurrentWeapon.Shoot()) {
            OnWeaponFire?.Invoke();
            hud.UpdateAmmo(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity, ReserveAmmo());
            look.Recoil();
            rifleIsFiring = true;
            if( rifleFire != null ) {
                this.StopCoroutine(rifleFire);
            }
            rifleFire = RifleFire(delayBetweenShots);
            this.StartCoroutine( rifleFire );
        }
    }

    bool rifleIsFiring = false;
    IEnumerator rifleFire;
    IEnumerator RifleFire( float delay ) {
        float timer = 0f;
        while( rifleIsFiring ) {
            if( timer < delay ) {
                timer += Time.deltaTime;
            } else {
                if (CurrentWeapon.Shoot()) {
                    OnWeaponFire?.Invoke();
                    hitscanWeapon.Fire(Mathf.RoundToInt(CurrentWeapon.damagePerShot));
                    hud.UpdateAmmo(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity, ReserveAmmo());
                    look.Recoil();
                    timer = 0f;
                } else {
                    rifleIsFiring = false;
                }
            }
            yield return null;
        }
    }

    void OnReload( InputAction.CallbackContext ctx ) {
        if( !isReloading ) {
            Reload();
        }
    }

    bool isReloading = false;
    void Reload() {
        if( ReserveAmmo() > 0 ) {
            StartReloadDelay();
        } else {
            return;
        }
    }

    public float ReloadDelay => (CurrentWeapon.ReloadDelayMultiplier() * 1f);
    void StartReloadDelay() {
        rifleIsFiring = false;
        fireRateCanShoot = true;
        shootDelay = null;

        if( reloadDelay != null ) {
            this.StopCoroutine(reloadDelay);
        }
        reloadDelay = ReloadDelayRoutine(ReloadDelay);
        this.StartCoroutine(reloadDelay);
    }

    IEnumerator reloadDelay;
    IEnumerator ReloadDelayRoutine( float delay ) {
        float timer = 0f;
        isReloading = true;
        OnWeaponStartReload?.Invoke();
        while( timer < delay ) {
            timer += Time.deltaTime;
            yield return null;
        }
        isReloading = false;
        UpdateReserve( CurrentWeapon.Reload( ReserveAmmo() ) );
        hud.UpdateAmmo(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity, ReserveAmmo());
        OnWeaponEndReload?.Invoke();
    }

    int ReserveAmmo() {
        switch( CurrentWeapon.type ) {
            case GunType.Energy:
                return energyAmmo;
            case GunType.Rocket:
                return rocketAmmo;
            case GunType.Rifle:
                return rifleAmmo;
            case GunType.Shotgun:
                return shotgunAmmo;
            default: case GunType.Pistol:
                return pistolAmmo;
        }
    }

    void UpdateReserve( int delta ) {
        UpdateReserve(CurrentWeapon.type, delta);
    }
    void UpdateReserve( GunType type, int delta ) {
        switch( type ) {
            case GunType.Energy:
                energyAmmo = SaturateAdd(energyAmmo, -delta);
                break;
            case GunType.Rocket:
                rocketAmmo = SaturateAdd(rocketAmmo, -delta);
                break;
            case GunType.Rifle:
                rifleAmmo = SaturateAdd(rifleAmmo, -delta);
                break;
            case GunType.Shotgun:
                shotgunAmmo = SaturateAdd(shotgunAmmo, -delta);
                break;
            default: case GunType.Pistol:
                pistolAmmo = SaturateAdd(pistolAmmo, -delta);
                break;
        }
    }

    int SaturateAdd( int a, int b ) {
        return Mathf.Clamp(a + b, 0, 999);
    }

    private void OnEnable()
    {
        fire.performed += OnFire;
        fire.canceled += OnFireUp;
        switchWeapon.performed += OnSwitchWeapon;
        reloadWeapon.performed += OnReload;
    }

    private void OnDisable()
    {
        fire.performed -= OnFire;
        fire.canceled -= OnFireUp;
        switchWeapon.performed -= OnSwitchWeapon;
        reloadWeapon.performed -= OnReload;
    }
}
