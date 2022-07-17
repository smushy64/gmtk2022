using UnityEngine;

public class GunData
{
    public string name { get; private set; }
    public GunType type { get; private set; }

    public GunQuality quality { get; private set; }

    public DamageEffect damageEffect { get; private set; }

    public float damagePerShot { get; private set; }
    public float delayBetweenShots { get; private set; }

    public int ammoCount = 0;

    public int magazineCapacity { get; private set; }
    public int pelletsPerShot { get; private set; }
    public int ammoConsumptionPerShot { get; private set; }

    public float recoilMultiplier { get; private set; }
    
    /// <summary>
    /// returns how much ammo is taken from reserve
    /// </summary>
    /// <param name="reserve"></param>
    /// <returns></returns>
    public int Reload( int reserve ) {
        int result = Mathf.Clamp(this.magazineCapacity - this.ammoCount, 0, reserve);
        this.ammoCount = this.ammoCount + result;
        return result;
    }

    /// <summary>
    /// returns if shoot is possible, if it is then the ammo count is reduced by ammo consumption per shot
    /// </summary>
    /// <returns></returns>
    public bool Shoot() {
        if( ammoCount >= ammoConsumptionPerShot ) {
            ammoCount -= ammoConsumptionPerShot;
            return true;
        }
        else {
            return false;
        }
    }

    public bool IsHitScan() {
        return this.type != GunType.Rocket && this.type != GunType.Energy;
    }

    public bool IsRifle() {
        return this.type == GunType.Rifle;
    }

    public bool IsShotgun() {
        return this.type == GunType.Shotgun;
    }

    public bool IsProjectile() {
        return this.type == GunType.Rocket;
    }

    public float ReloadDelayMultiplier() {
        switch( this.type ) {
            case GunType.Shotgun: return 1.15f;
            case GunType.Rocket: return 1.5f;
            case GunType.Energy: return 1.1f;
            default: return 1f;
        }
    }

    public GunData() {
        this.quality = GenerateGunQuality();
        this.type = (GunType)Random.Range(0, 4);

        this.name = this.quality.ToString() + " " + GunTypeText(this.type);

        Vector2 range = dataRanges.GetDamagePerShotRange( this.quality );
        this.damagePerShot = Random.Range(range.x, range.y) *
            (this.type == GunType.Shotgun ? dataRanges.shotgunDamagePerShotMultiplier : 1f);

        range = dataRanges.GetDelayBetweenShotsMultiplierRange( this.quality );
        this.delayBetweenShots = Random.Range(range.x, range.y) *
            (
                this.type == GunType.Rifle ? dataRanges.rifleFireRateMultipler :
                ( this.type == GunType.Pistol ? dataRanges.pistolFireRateMultiplier : 1f )
            );

        Vector2Int rangeInt = dataRanges.GetAmmoConsumptionPerShotRange( this.quality );
        this.ammoConsumptionPerShot = Random.Range(rangeInt.x, rangeInt.y);

        rangeInt = dataRanges.GetMagazineCapacityRange( this.quality );
        this.magazineCapacity = Random.Range(rangeInt.x, rangeInt.y);
        int mod = this.magazineCapacity % this.ammoConsumptionPerShot;
        if( mod != 0 ) {
            this.magazineCapacity += mod;
        }

        rangeInt = dataRanges.GetPelletsPerShotRange( this.quality );
        this.pelletsPerShot = (this.type == GunType.Shotgun) ? Random.Range(rangeInt.x, rangeInt.y) : 1;


        range = dataRanges.GetRecoilMultiplierRange(this.quality);
        this.recoilMultiplier = Random.Range(range.x, range.y);

        this.ammoCount = this.magazineCapacity;
    }

    public override string ToString()
    {
        return this.name + "\n" +
        "Damage: " + this.damagePerShot.ToString() + "\n" +
        "Magazine Capacity: " + this.magazineCapacity.ToString() + "\n" +
        ( this.type == GunType.Shotgun ? "Pellets per shot: " + this.pelletsPerShot.ToString() + "\n" : "\n" ) +
        "Ammo Consumption per shot: " + this.ammoConsumptionPerShot.ToString() + "\n" +
        "Recoil Multiplier: " + this.recoilMultiplier.ToString() + "\n";
    }

    GunQuality GenerateGunQuality() {
        int generatedValue = Random.Range(0, 100);
        if( generatedValue == 0 )
            return GunQuality.Legendary;
        int step = 1;
        while( true ) {
            generatedValue = Random.Range(0, 100);
            switch( step ) {
                case 1:
                    if( generatedValue < (int)GunQuality.Incredible )
                        return GunQuality.Incredible;
                    break;
                case 2:
                    if( generatedValue < (int)GunQuality.Terrible )
                        return GunQuality.Terrible;
                    break;
                case 3:
                    if( generatedValue < (int)GunQuality.Mediocre )
                        return GunQuality.Mediocre;
                    break;
                case 4:
                    if( generatedValue < (int)GunQuality.Great )
                        return GunQuality.Great;
                    break;
                case 5:
                    if( generatedValue < (int)GunQuality.Good )
                        return GunQuality.Good;
                    break;
                default: case 6:
                    return GunQuality.Common;
            }
            ++step;
        }
    }

    static GunDataRanges dataRanges => _dataRanges == null ? Resources.Load<GunDataRanges>("GunDataRanges") : _dataRanges;
    static GunDataRanges _dataRanges;

    string GunTypeText(GunType type) {
        switch(type) {
            case GunType.Pistol: return "Pistol";
            case GunType.Shotgun: return "Shotgun";
            case GunType.Rifle: return "Rifle";
            case GunType.Rocket: return "Rocket Launcher";
            default: case GunType.Energy: return "Energy Gun";
        }
    }
}

public enum GunType
{
    // Semi
    Pistol,
    // Semi, delayed
    Shotgun,
    // Auto
    Rifle,
    // Semi, delayed
    Rocket,
    // Continuous
    Energy
}

public enum GunQuality
{
    Common,
    Good = 25,
    Great = 20,
    Mediocre = 15,
    Terrible = 10,
    Incredible = 5,
    Legendary = 1
}

/// <summary>
/// Stretch goal
/// </summary>
public enum DamageEffect
{
    Normal,
    Bleed,
    Fire,
    Electric
}
