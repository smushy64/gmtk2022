using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private GunData data;

    [SerializeField] private GameObject pistolPrefab;
    [SerializeField] private GameObject shotgunPrefab;
    [SerializeField] private GameObject riflePrefab;
    [SerializeField] private GameObject rocketLauncherPrefab;

    private ScoreManager SM;

    private void Awake()
    {
        SM = FindObjectOfType<ScoreManager>();
        UpdateGunData(new GunData(SM));
    }

    public void UpdateGunData(GunData data)
    {
        this.data = data;
        GenerateModel();
    }

    private void GenerateModel()
    {
        GameObject prefab;
        switch (data.type)
        {
            case GunType.Pistol:
                prefab = pistolPrefab;
                break;
            case GunType.Shotgun:
                prefab = shotgunPrefab;
                break;
            case GunType.Rifle:
                prefab = riflePrefab;
                break;
            case GunType.Rocket:
                prefab = rocketLauncherPrefab;
                break;
            default:
                Debug.LogError("Invalid weapon type assigned to this gun");
                return;
        }

        if (prefab != null)
        {
            Transform model = Instantiate(prefab, transform).transform;
            model.localScale = Vector3.one * 0.1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var weaponManager = other.GetComponentInChildren<WeaponManager>();

            if (weaponManager != null)
            {
                weaponManager.SwapWeapon(data);
                weaponManager.ChangeAmmoCount(data.type, -data.magazineCapacity);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("[WeaponPickup] No weapon manager detected on player.");
            }

        }
    }
}
