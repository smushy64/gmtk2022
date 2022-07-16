using UnityEngine;

public class GunData
{
    public string name { get; private set; }
    public GunType type { get; private set; }

    public GunQuality quality { get; private set; }

    public DamageEffect damageEffect { get; private set; }

    public float damagePerShot { get; private set; }

    public int magazineCapacity { get; private set; }
    public int pelletsPerShot { get; private set; }
    public int ammoConsumptionPerShot { get; private set; }

    public float recoilMultiplier { get; private set; }
    
    public GunData() {
        this.quality = GenerateGunQuality();
        this.type = (GunType)Random.Range(0, 5);

        this.name = this.quality.ToString() + " " + GunTypeText(this.type);

        Vector2 range = dataRanges.GetDamagePerShotRange( this.quality );
        this.damagePerShot = Random.Range(range.x, range.y) *
            (this.type == GunType.Shotgun ? dataRanges.shotgunDamagePerShotMultiplier : 1f);

        Vector2Int rangeInt = dataRanges.GetMagazineCapacityRange( this.quality );
        this.magazineCapacity = Random.Range(rangeInt.x, rangeInt.y);

        rangeInt = dataRanges.GetPelletsPerShotRange( this.quality );
        this.pelletsPerShot = (this.type == GunType.Shotgun) ? Random.Range(rangeInt.x, rangeInt.y) : 1;

        rangeInt = dataRanges.GetAmmoConsumptionPerShotRange( this.quality );
        this.ammoConsumptionPerShot = Random.Range(rangeInt.x, rangeInt.y);

        range = dataRanges.GetRecoilMultiplierRange(this.quality);
        this.recoilMultiplier = Random.Range(range.x, range.y);
    }

    public GunData( GunQuality quality ) {
        this.quality = quality;
        this.type = (GunType)Random.Range(0, 5);

        this.name = this.quality.ToString() + " " + GunTypeText(this.type);

        Vector2 range = dataRanges.GetDamagePerShotRange( this.quality );
        this.damagePerShot = Random.Range(range.x, range.y) *
            (this.type == GunType.Shotgun ? dataRanges.shotgunDamagePerShotMultiplier : 1f);

        Vector2Int rangeInt = dataRanges.GetMagazineCapacityRange( this.quality );
        this.magazineCapacity = Random.Range(rangeInt.x, rangeInt.y);

        rangeInt = dataRanges.GetPelletsPerShotRange( this.quality );
        this.pelletsPerShot = (this.type == GunType.Shotgun) ? Random.Range(rangeInt.x, rangeInt.y) : 1;

        rangeInt = dataRanges.GetAmmoConsumptionPerShotRange( this.quality );
        this.ammoConsumptionPerShot = Random.Range(rangeInt.x, rangeInt.y);

        range = dataRanges.GetRecoilMultiplierRange(this.quality);
        this.recoilMultiplier = Random.Range(range.x, range.y);
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
