using System.Collections;
using UnityEngine;

public class GunAnimator : MonoBehaviour
{

    int currentWeapon {
        get {
            switch( weapons.CurrentWeapon.type ) {
                case GunType.Shotgun:
                    return 1;
                case GunType.Rifle:
                    return 2;
                case GunType.Rocket:
                    return 3;
                default: case GunType.Pistol:
                    return 0;
            }
        }
    }

    [SerializeField]
    Animator pistol, shotgun;
    [SerializeField]
    GameObject rifle, rocketLauncher;
    [SerializeField]
    SoundEffectPlayer pistolSFX, shotgunSFX, shotgunPumpSFX, rifleSFX, rocketSFX, reloadSFX, switchSFX, pickupSFX;
    [SerializeField]
    ParticleSystem pistolFlash, shotgunFlash, rifleFlash, rocketLauncherFlash;
    [SerializeField]
    ParticleSystem pistolCasing, shotgunCasing, rifleCasing;
    [SerializeField]
    MuzzleFlashLight muzzleFlashLight;

    [SerializeField]
    WeaponManager weapons;

    void OnWeaponReload() {
        this.StopAllCoroutines();
        reloadAnimation = SwitchAnimation( weapons.ReloadDelay );
        this.StartCoroutine(reloadAnimation);
        reloadSFX.Play();
    }
    
    void OnWeaponPickup() {
        pickupSFX.Play();
    }

    void OnWeaponSwitchStart() {
        this.StopAllCoroutines();
        switchAnimation = SwitchAnimation( WeaponManager.WEAPON_SWITCH_DELAY );
        this.StartCoroutine(switchAnimation);
        switchSFX.Play();
    }

    IEnumerator reloadAnimation;
    IEnumerator switchAnimation;
    IEnumerator SwitchAnimation( float length ) {
        float timer = 0f;
        float total = length / 2f;
        Quaternion targetRotation = Quaternion.AngleAxis(90f, Vector3.right);
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;
        while( timer < total ) {
            transform.localRotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                timer / total
            );

            transform.localPosition = Vector3.Lerp(
                startPosition,
                Vector3.back,
                timer / total
            );

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = targetRotation;
        transform.localPosition = Vector3.back;
        timer = 0f;
        while( timer < total ) {
            transform.localRotation = Quaternion.Slerp(
                targetRotation,
                Quaternion.identity,
                timer / total
            );

            transform.localPosition = Vector3.Lerp(
                Vector3.back,
                Vector3.zero,
                timer / total
            );

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }

    void OnWeaponFire() {
        if( shoot != null )
            this.StopCoroutine(shoot);
        shoot = Shoot();
        this.StartCoroutine(shoot);

        if( shootAnimation != null )
            this.StopCoroutine(shootAnimation);
        shootAnimation = ShootAnimation();
        this.StartCoroutine(shootAnimation);

        muzzleFlashLight.Flash();

        if( currentWeapon == 0 ) {
            pistol.SetBool("isShooting", true);
            pistolSFX.Play();
            pistolFlash.Play();
            pistolCasing.Play();
        } else if ( currentWeapon == 1 ) {
            shotgun.SetBool("isShooting", true);
            shotgunSFX.Play();
            shotgunFlash.Play();
            PlayPumpSFX();
        } else if ( currentWeapon == 2 ) {
            rifleSFX.Play();
            rifleFlash.Play();
            rifleCasing.Play();
        } else if ( currentWeapon == 3 ) {
            rocketSFX.Play();
            rocketLauncherFlash.Play();
        }

    }
    void PlayPumpSFX() {
        if( playPumpSFXRoutine != null ) {
            this.StopCoroutine(playPumpSFXRoutine);
        }
        playPumpSFXRoutine = PlayPumpSFXRoutine();
        this.StartCoroutine(playPumpSFXRoutine);
    }
    IEnumerator playPumpSFXRoutine;
    IEnumerator PlayPumpSFXRoutine() {
        yield return new WaitForSeconds(0.12f);
        shotgunPumpSFX.Play();
        shotgunCasing.Play();
    }

    IEnumerator shootAnimation;
    IEnumerator ShootAnimation() {
        yield return new WaitForSeconds( 0.01f );
        float timer = 0f;
        float length = weapons.CurrentWeapon.delayBetweenShots - 0.01f;

        Vector3 kickBackPosition = Vector3.back * 0.1f;
        Quaternion kickBackRotation = Quaternion.AngleAxis(-25f, Vector3.right);
        transform.localPosition = kickBackPosition;
        transform.localRotation = kickBackRotation;

        while( timer < length ) {
            float t = timer / length;
            transform.localPosition = Vector3.Lerp(
                kickBackPosition,
                Vector3.zero,
                t
            );
            transform.localRotation = Quaternion.Lerp(
                kickBackRotation,
                Quaternion.identity,
                t
            );
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    IEnumerator shoot;
    IEnumerator Shoot() {
        yield return new WaitForSeconds(0.0001f);
        if( currentWeapon == 0 ) {
            pistol.SetBool("isShooting", false);
        } else if ( currentWeapon == 1 ) {
            shotgun.SetBool("isShooting", false);
        }
    }

    void SwitchWeaponModel() {
        if( weapons.CurrentWeapon == null )
            return;
        pistol.gameObject.SetActive(false);
        shotgun.gameObject.SetActive(false);
        rifle.SetActive(false);
        rocketLauncher.SetActive(false);
        switch( weapons.CurrentWeapon.type ) {
            case GunType.Pistol:
                pistol.gameObject.SetActive(true);
                break;
            case GunType.Shotgun:
                shotgun.gameObject.SetActive(true);
                break;
            case GunType.Rifle:
                rifle.SetActive(true);
                break;
            case GunType.Rocket:
                rocketLauncher.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void Start() {
        SwitchWeaponModel();
    }

    private void OnEnable() {
        weapons.OnWeaponSwitch += SwitchWeaponModel;
        weapons.OnWeaponStartSwitch += OnWeaponSwitchStart;
        weapons.OnWeaponFire += OnWeaponFire;
        weapons.OnWeaponStartReload += OnWeaponReload;
        weapons.OnWeaponPickup += OnWeaponPickup;
    }
    private void OnDisable() {
        weapons.OnWeaponSwitch -= SwitchWeaponModel;
        weapons.OnWeaponStartSwitch -= OnWeaponSwitchStart;
        weapons.OnWeaponFire -= OnWeaponFire;
        weapons.OnWeaponStartReload -= OnWeaponReload;
    }

}
