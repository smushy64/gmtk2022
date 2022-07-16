using UnityEngine;

[CreateAssetMenu( menuName = "Gun Data Ranges" )]
public class GunDataRanges : ScriptableObject
{

    public Vector2 GetDamagePerShotRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return goodDamagePerShotRange;
            case GunQuality.Great:           return greatDamagePerShotRange;
            case GunQuality.Terrible:        return terribleDamagePerShotRange;
            case GunQuality.Mediocre:        return mediocreDamagePerShotRange;
            case GunQuality.Incredible:      return incredibleDamagePerShotRange;
            case GunQuality.Legendary:       return legendaryDamagePerShotRange;
            default: case GunQuality.Common: return commonDamagePerShotRange;
        }
    }
    public Vector2Int GetMagazineCapacityRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return goodMagazineCapacityRange;
            case GunQuality.Great:           return greatMagazineCapacityRange;
            case GunQuality.Terrible:        return terribleMagazineCapacityRange;
            case GunQuality.Mediocre:        return mediocreMagazineCapacityRange;
            case GunQuality.Incredible:      return incredibleMagazineCapacityRange;
            case GunQuality.Legendary:       return legendaryMagazineCapacityRange;
            default: case GunQuality.Common: return commonMagazineCapacityRange;
        }
    }
    public Vector2Int GetPelletsPerShotRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return goodPelletsPerShotRange;
            case GunQuality.Great:           return greatPelletsPerShotRange;
            case GunQuality.Terrible:        return terriblePelletsPerShotRange;
            case GunQuality.Mediocre:        return mediocrePelletsPerShotRange;
            case GunQuality.Incredible:      return incrediblePelletsPerShotRange;
            case GunQuality.Legendary:       return legendaryPelletsPerShotRange;
            default: case GunQuality.Common: return commonPelletsPerShotRange;
        }
    }
    public Vector2Int GetAmmoConsumptionPerShotRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return goodAmmoConsumptionPerShot;
            case GunQuality.Great:           return greatAmmoConsumptionPerShot;
            case GunQuality.Terrible:        return terribleAmmoConsumptionPerShot;
            case GunQuality.Mediocre:        return mediocreAmmoConsumptionPerShot;
            case GunQuality.Incredible:      return incredibleAmmoConsumptionPerShot;
            case GunQuality.Legendary:       return legendaryAmmoConsumptionPerShot;
            default: case GunQuality.Common: return commonAmmoConsumptionPerShot;
        }
    }
    public Vector2 GetRecoilMultiplierRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return goodRecoilMultiplier;
            case GunQuality.Great:           return greatRecoilMultiplier;
            case GunQuality.Terrible:        return terribleRecoilMultiplier;
            case GunQuality.Mediocre:        return mediocreRecoilMultiplier;
            case GunQuality.Incredible:      return incredibleRecoilMultiplier;
            case GunQuality.Legendary:       return legendaryRecoilMultiplier;
            default: case GunQuality.Common: return commonRecoilMultiplier;
        }
    }

    [Header("Shotgun Damage Per Shot Multiplier")]
    public float shotgunDamagePerShotMultiplier = 0.2f;

    [Header("Common =================")]
    public Vector2    commonDamagePerShotRange;
    public Vector2Int commonMagazineCapacityRange;
    public Vector2Int commonPelletsPerShotRange;
    public Vector2Int commonAmmoConsumptionPerShot;
    public Vector2    commonRecoilMultiplier;
    [Space]

    [Header("Good =================")]
    public Vector2    goodDamagePerShotRange;
    public Vector2Int goodMagazineCapacityRange;
    public Vector2Int goodPelletsPerShotRange;
    public Vector2Int goodAmmoConsumptionPerShot;
    public Vector2    goodRecoilMultiplier;
    [Space]

    [Header("Great =================")]
    public Vector2    greatDamagePerShotRange;
    public Vector2Int greatMagazineCapacityRange;
    public Vector2Int greatPelletsPerShotRange;
    public Vector2Int greatAmmoConsumptionPerShot;
    public Vector2    greatRecoilMultiplier;
    [Space]

    [Header("Mediocre =================")]
    public Vector2    mediocreDamagePerShotRange;
    public Vector2Int mediocreMagazineCapacityRange;
    public Vector2Int mediocrePelletsPerShotRange;
    public Vector2Int mediocreAmmoConsumptionPerShot;
    public Vector2    mediocreRecoilMultiplier;
    [Space]

    [Header("Terrible =================")]
    public Vector2    terribleDamagePerShotRange;
    public Vector2Int terribleMagazineCapacityRange;
    public Vector2Int terriblePelletsPerShotRange;
    public Vector2Int terribleAmmoConsumptionPerShot;
    public Vector2    terribleRecoilMultiplier;
    [Space]

    [Header("Incredible =================")]
    public Vector2    incredibleDamagePerShotRange;
    public Vector2Int incredibleMagazineCapacityRange;
    public Vector2Int incrediblePelletsPerShotRange;
    public Vector2Int incredibleAmmoConsumptionPerShot;
    public Vector2    incredibleRecoilMultiplier;
    [Space]

    [Header("Legendary =================")]
    public Vector2    legendaryDamagePerShotRange;
    public Vector2Int legendaryMagazineCapacityRange;
    public Vector2Int legendaryPelletsPerShotRange;
    public Vector2Int legendaryAmmoConsumptionPerShot;
    public Vector2    legendaryRecoilMultiplier;

}
