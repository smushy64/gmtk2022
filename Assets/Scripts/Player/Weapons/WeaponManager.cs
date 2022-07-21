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
    UIHud hud;

    [Header("Debug ======================")]
    [SerializeField]
    bool spawnWithWeapons = false;

    PlayerMovement movement;
    PlayerInput input;
    PlayerLook look;
    UIAimBar aimBar;

    InputAction fire;
    InputAction reloadWeapon;
    InputAction switchWeapon;
    InputAction switchWeapon0;
    InputAction switchWeapon1;
    InputAction switchWeapon2;
    InputAction switchWeapon3;

    public Action OnWeaponStartSwitch;
    public Action OnWeaponSwitch;
    public Action OnWeaponEndSwitch;
    public Action OnWeaponFire;
    public Action OnWeaponStartReload;
    public Action OnWeaponEndReload;
    public Action OnWeaponPickup;

    int pistolAmmo  = 20;
    int shotgunAmmo = 5;
    int rifleAmmo   = 25;
    int rocketAmmo  = 3;
    int energyAmmo  = 40;

    int currentWeaponIndex = 0;
    const int MAX_WEAPON_COUNT = 4;
    GunData[] weapons = new GunData[MAX_WEAPON_COUNT];
    bool weaponsContainNull = true;
    int weaponCount = 0;

    public GunData CurrentWeapon => weapons[currentWeaponIndex];
    float recoilMagnitude = 1f;

    private void Awake()
    {
        input = GetComponentInParent<PlayerInput>();
        look = GetComponentInParent<PlayerLook>();
        movement = GetComponentInParent<PlayerMovement>();
        aimBar = FindObjectOfType<UIAimBar>();
        fire = input.actions["Fire"];
        switchWeapon = input.actions["ScrollWeapon"];
        switchWeapon0 = input.actions["SwitchWeapon0"];
        switchWeapon1 = input.actions["SwitchWeapon1"];
        switchWeapon2 = input.actions["SwitchWeapon2"];
        switchWeapon3 = input.actions["SwitchWeapon3"];
        reloadWeapon = input.actions["Reload"];
        hud = FindObjectOfType<UIHud>();

        CheckForEmptyWeaponSlot();

        movement.OnCrouch += OnCrouch;
        movement.OnAirborne += OnAirborneOrSlide;
        movement.OnSlide += OnAirborneOrSlide;
        movement.OnGrounded += OnGrounded;

        if( spawnWithWeapons ) {
            for (int i = 0; i < MAX_WEAPON_COUNT; ++i) {
                weapons[i] = new GunData();
            }
            weaponsContainNull = false;
            weaponCount = 4;
            hud.Weapons.UpdateList(weapons);
            hud.UpdateAmmoCounter(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity);
            hud.UpdateReserveCounters(pistolAmmo, shotgunAmmo, rifleAmmo, rocketAmmo);
            hud.UpdateWeaponName(CurrentWeapon.name);
        }
        else {
            hud.NoWeapon();
        }
    }

    void OnCrouch() {
        recoilMagnitude = 0.7f;
        aimBar.SetDelta(-0.25f);
    }
    void OnAirborneOrSlide() {
        recoilMagnitude = 1.2f;
        aimBar.SetDelta(0.5f);
    }
    void OnGrounded() {
        if( movement.IsMoving ) {
            recoilMagnitude = 1f;
            aimBar.SetDelta(0.25f);
        } else {
            recoilMagnitude = 0.9f;
            aimBar.SetDelta(0f);
        }
    }

    public void UpdateAmmoCount( GunType type, int delta ) {
        AddToReserve(type, delta);
        if( CurrentWeapon != null ) {
            hud.UpdateAmmoCounter(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity);
            hud.UpdateReserveCounters(pistolAmmo, shotgunAmmo, rifleAmmo, rocketAmmo);
        }
    }

    /// <summary>
    /// returns weapon data if weapon swapped, otherwise null if weapon added
    /// </summary>
    /// <param name="newWeapon"></param>
    /// <returns></returns>
    public GunData PickupWeapon( GunData newWeapon ) {
        GunData returnWeapon = null;
        if( weaponsContainNull ) {
            for (int i = 0; i < weapons.Length; ++i){
                if ( weapons[i] == null ) {
                    weapons[i] = newWeapon;
                    currentWeaponIndex = i;
                    if( i == weapons.Length - 1 ) {
                        weaponsContainNull = false;
                    }
                    break;
                }
            }
            weaponCount++;
        } else {
            returnWeapon = weapons[currentWeaponIndex];
            weapons[currentWeaponIndex] = newWeapon;
        }

        UpdateSelectedWeapon();
        OnWeaponPickup?.Invoke();
        hud.Weapons.UpdateList(weapons);
        return returnWeapon;
    }

    private void CheckForEmptyWeaponSlot()
    {
        weaponsContainNull = false;
        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i] == null)
            {
                weaponsContainNull = true;
            }
        }
    }

    void OnSwitchWeapon(InputAction.CallbackContext ctx)
    {
        if( PauseMenu.Paused || weaponCount <= 1 )
            return;
        SwitchWeapon((ctx.ReadValue<float>() > 0f) ? 1 : -1);
        UpdateSelectedWeapon();
    }
    void OnSwitchWeapon0(InputAction.CallbackContext ctx)
    {
        if( PauseMenu.Paused || weaponCount <= 1 )
            return;
        if( SwitchWeaponAtIndex(0) )
            UpdateSelectedWeapon();
    }
    void OnSwitchWeapon1(InputAction.CallbackContext ctx)
    {
        if( PauseMenu.Paused || weaponCount <= 1 )
            return;
        if( SwitchWeaponAtIndex(1) )
            UpdateSelectedWeapon();
    }
    void OnSwitchWeapon2(InputAction.CallbackContext ctx)
    {
        if( PauseMenu.Paused || weaponCount <= 1 )
            return;
        if( SwitchWeaponAtIndex(2) )
            UpdateSelectedWeapon();
    }
    void OnSwitchWeapon3(InputAction.CallbackContext ctx)
    {
        if( PauseMenu.Paused || weaponCount <= 1 )
            return;
        if( SwitchWeaponAtIndex(3) )
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
            hud.Weapons.UpdateSelected(currentWeaponIndex);
            hud.UpdateAmmoCounter(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity);
            hud.UpdateWeaponName(CurrentWeapon.name);
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

    /// <summary>
    /// returns true if operation was successful
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    bool SwitchWeaponAtIndex(int index) {
        if(index == currentWeaponIndex)
            return false;
        int lastIndex = currentWeaponIndex;
        currentWeaponIndex = index;
        if(CurrentWeapon == null) {
            currentWeaponIndex = lastIndex;
            return false;
        }
        return true;
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
        if( CurrentWeapon == null || isReloading || isSwitching || PauseMenu.Paused )
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
                        projectileWeapon.Fire(CurrentWeapon.damagePerShot);
                        ShootDelay(CurrentWeapon.delayBetweenShots);
                        hud.UpdateAmmoCounter(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity);
                        look.Recoil(CurrentWeapon.recoilMultiplier * recoilMagnitude);
                    }
                }
                break;
            case GunType.Shotgun:
                if (fireRateCanShoot) {
                    if( CurrentWeapon.Shoot() ) {
                        OnWeaponFire?.Invoke();
                        hitscanWeapon.Fire( CurrentWeapon.pelletsPerShot, CurrentWeapon.damagePerShot);
                        ShootDelay(CurrentWeapon.delayBetweenShots);
                        hud.UpdateAmmoCounter(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity);
                        look.Recoil(CurrentWeapon.recoilMultiplier * recoilMagnitude);
                    }
                }
                break;
            default:
                if(fireRateCanShoot) {
                    if (CurrentWeapon.Shoot()) {
                        OnWeaponFire?.Invoke();
                        hitscanWeapon.Fire( CurrentWeapon.damagePerShot );
                        ShootDelay(CurrentWeapon.delayBetweenShots);
                        hud.UpdateAmmoCounter(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity);
                        look.Recoil(CurrentWeapon.recoilMultiplier * recoilMagnitude);
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
            hud.UpdateAmmoCounter(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity);
            look.Recoil( CurrentWeapon.recoilMultiplier * recoilMagnitude );
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
                    hud.UpdateAmmoCounter(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity);
                    look.Recoil( CurrentWeapon.recoilMultiplier * recoilMagnitude );
                    timer = 0f;
                } else {
                    rifleIsFiring = false;
                }
            }
            yield return null;
        }
    }

    void OnReload( InputAction.CallbackContext ctx ) {
        if( PauseMenu.Paused || CurrentWeapon == null )
            return;
        if( !isReloading ) {
            Reload();
        }
    }

    bool isReloading = false;
    void Reload() {
        if( ReserveAmmo() > 0 && CurrentWeapon.ammoCount < CurrentWeapon.magazineCapacity ) {
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
        AddToReserve( -CurrentWeapon.Reload( ReserveAmmo() ) );
        hud.UpdateAmmoCounter(CurrentWeapon.ammoCount, CurrentWeapon.magazineCapacity);
        hud.UpdateReserveCounters(pistolAmmo, shotgunAmmo, rifleAmmo, rocketAmmo);
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

    void AddToReserve( int delta ) {
        AddToReserve(CurrentWeapon.type, delta);
    }
    void AddToReserve( GunType type, int delta ) {
        switch( type ) {
            case GunType.Energy:
                energyAmmo = SaturateAdd(energyAmmo, delta);
                break;
            case GunType.Rocket:
                rocketAmmo = SaturateAdd(rocketAmmo, delta);
                break;
            case GunType.Rifle:
                rifleAmmo = SaturateAdd(rifleAmmo, delta);
                break;
            case GunType.Shotgun:
                shotgunAmmo = SaturateAdd(shotgunAmmo, delta);
                break;
            default: case GunType.Pistol:
                pistolAmmo = SaturateAdd(pistolAmmo, delta);
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
        switchWeapon0.performed += OnSwitchWeapon0;
        switchWeapon1.performed += OnSwitchWeapon1;
        switchWeapon2.performed += OnSwitchWeapon2;
        switchWeapon3.performed += OnSwitchWeapon3;
        reloadWeapon.performed += OnReload;
    }

    private void OnDisable()
    {
        fire.performed -= OnFire;
        fire.canceled -= OnFireUp;
        switchWeapon.performed -= OnSwitchWeapon;
        switchWeapon0.performed -= OnSwitchWeapon0;
        switchWeapon1.performed -= OnSwitchWeapon1;
        switchWeapon2.performed -= OnSwitchWeapon2;
        switchWeapon3.performed -= OnSwitchWeapon3;
        reloadWeapon.performed -= OnReload;
    }
}
