using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class HitscanWeapon : MonoBehaviour
{
    [SerializeField] private Ammo.Type type;
    [SerializeField] private int maxAmmoPerMagazine = 10;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float reloadDuration = 1f;
    [SerializeField] private float spread = 0f;
    [SerializeField] private int pelletsPerShot = 1;
    private WeaponAnimator animator;
    private float timeSinceLastShot = 0f;
    private int ammoInMag = 0;

    public Ammo.Type Type => type;
    public bool CanFire => timeSinceLastShot >= 1f / fireRate;
    public bool CanReload => AmmoLeftAsPercentage < 1f;
    public int MaxAmmo => maxAmmoPerMagazine;
    public int AmmoLeft => ammoInMag;
    public float AmmoLeftAsPercentage => ammoInMag / (float)maxAmmoPerMagazine;

    private void Awake()
    {
        animator = GetComponent<WeaponAnimator>();
        ammoInMag = maxAmmoPerMagazine;
    }

    private void OnEnable()
    {
        // Bypass firerate cooldown if we're just pulling this weapon out.
        timeSinceLastShot = 1f / fireRate;
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    public void Fire(Vector3 origin, Vector3 direction)
    {
        timeSinceLastShot = 0f;
        ammoInMag--;

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 dir = CalculateSpread(direction);
            Ray ray = new Ray(origin, dir);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Debug.DrawLine(origin, hit.point, Color.blue, 3f);
            }
            else
            {
                Debug.DrawRay(origin, dir * 100f, Color.red, 3f);
            }
        }
    }

    private Vector3 CalculateSpread(Vector3 dir)
    {
        Vector3 rotatedDir = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, spread), Vector3.up) * dir;
        return Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), dir) * rotatedDir;
    }

    public void AddAmmo(int amount) => ammoInMag += amount;

    public void StartReload(Action onReloadFinished)
    {
        animator.StartReload(reloadDuration, onReloadFinished);
    }

    public void Equip(Action onComplete = null)
    {
        animator.StartEquip(onComplete);
    }

    public void Unequip(Action onComplete = null)
    {
        animator.StartUnequip(onComplete);
    }
}
