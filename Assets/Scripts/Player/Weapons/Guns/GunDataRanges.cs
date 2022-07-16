using UnityEngine;

[CreateAssetMenu( menuName = "Gun Data Ranges" )]
public class GunDataRanges : ScriptableObject
{

    public Vector2 GetDamagePerShotRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return good.DamagePerShot;
            case GunQuality.Great:           return great.DamagePerShot;
            case GunQuality.Terrible:        return terrible.DamagePerShot;
            case GunQuality.Mediocre:        return mediocre.DamagePerShot;
            case GunQuality.Incredible:      return incredible.DamagePerShot;
            case GunQuality.Legendary:       return legendary.DamagePerShot;
            default: case GunQuality.Common: return common.DamagePerShot;
        }
    }
    public Vector2Int GetMagazineCapacityRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return good.MagazineCapacity;
            case GunQuality.Great:           return great.MagazineCapacity;
            case GunQuality.Terrible:        return terrible.MagazineCapacity;
            case GunQuality.Mediocre:        return mediocre.MagazineCapacity;
            case GunQuality.Incredible:      return incredible.MagazineCapacity;
            case GunQuality.Legendary:       return legendary.MagazineCapacity;
            default: case GunQuality.Common: return common.MagazineCapacity;
        }
    }
    public Vector2Int GetPelletsPerShotRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return good.PelletsPerShot;
            case GunQuality.Great:           return great.PelletsPerShot;
            case GunQuality.Terrible:        return terrible.PelletsPerShot;
            case GunQuality.Mediocre:        return mediocre.PelletsPerShot;
            case GunQuality.Incredible:      return incredible.PelletsPerShot;
            case GunQuality.Legendary:       return legendary.PelletsPerShot;
            default: case GunQuality.Common: return common.PelletsPerShot;
        }
    }
    public Vector2Int GetAmmoConsumptionPerShotRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return good.AmmoConsumptionPerShot;
            case GunQuality.Great:           return great.AmmoConsumptionPerShot;
            case GunQuality.Terrible:        return terrible.AmmoConsumptionPerShot;
            case GunQuality.Mediocre:        return mediocre.AmmoConsumptionPerShot;
            case GunQuality.Incredible:      return incredible.AmmoConsumptionPerShot;
            case GunQuality.Legendary:       return legendary.AmmoConsumptionPerShot;
            default: case GunQuality.Common: return common.AmmoConsumptionPerShot;
        }
    }
    public Vector2 GetRecoilMultiplierRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return good.RecoilMultiplier;
            case GunQuality.Great:           return great.RecoilMultiplier;
            case GunQuality.Terrible:        return terrible.RecoilMultiplier;
            case GunQuality.Mediocre:        return mediocre.RecoilMultiplier;
            case GunQuality.Incredible:      return incredible.RecoilMultiplier;
            case GunQuality.Legendary:       return legendary.RecoilMultiplier;
            default: case GunQuality.Common: return common.RecoilMultiplier;
        }
    }
    public Vector2 GetDelayBetweenShotsMultiplierRange( GunQuality quality ) {
        switch( quality ) {
            case GunQuality.Good:            return good.DelayBetweenShots;
            case GunQuality.Great:           return great.DelayBetweenShots;
            case GunQuality.Terrible:        return terrible.DelayBetweenShots;
            case GunQuality.Mediocre:        return mediocre.DelayBetweenShots;
            case GunQuality.Incredible:      return incredible.DelayBetweenShots;
            case GunQuality.Legendary:       return legendary.DelayBetweenShots;
            default: case GunQuality.Common: return common.DelayBetweenShots;
        }
    }

    [Header("Shotgun Damage Per Shot Multiplier")]
    public float shotgunDamagePerShotMultiplier = 0.2f;
    [Header("Rifle Fire Rate Multiplier")]
    public float rifleFireRateMultipler = 0.3f;
    [Header("Pistol Fire Rate Multiplier")]
    public float pistolFireRateMultiplier = 0.8f;

    [System.Serializable]
    public struct Ranges {
        public Vector2    DamagePerShot;
        public Vector2    DelayBetweenShots;
        public Vector2Int MagazineCapacity;
        public Vector2Int PelletsPerShot;
        public Vector2Int AmmoConsumptionPerShot;
        public Vector2    RecoilMultiplier;    
    }

    [Header("Common =================")]
    public Ranges common;
    [Space]

    [Header("Good =================")]
    public Ranges good;
    [Space]

    [Header("Great =================")]
    public Ranges great;
    [Space]

    [Header("Mediocre =================")]
    public Ranges mediocre;
    [Space]

    [Header("Terrible =================")]
    public Ranges terrible;
    [Space]

    [Header("Incredible =================")]
    public Ranges incredible;
    [Space]

    [Header("Legendary =================")]
    public Ranges legendary;

}
