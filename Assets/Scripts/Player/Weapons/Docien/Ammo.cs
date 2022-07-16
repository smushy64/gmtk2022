using UnityEngine;

public class Ammo : MonoBehaviour
{
    public enum Type
    {
        Shotgun,
        Pistol,
        Rifle
    }

    [SerializeField] private int maxShotgunReserve;
    [SerializeField] private int maxPistolReserve;
    [SerializeField] private int maxRifleReserve;
    private int shotgunAmmoReserve = 0;
    private int pistolAmmoReserve = 0;
    private int rifleAmmoReserve = 0;

    private void Awake()
    {
        shotgunAmmoReserve = maxShotgunReserve;
        pistolAmmoReserve = maxPistolReserve;
        rifleAmmoReserve = maxRifleReserve;
    }

    // Passes in the amount of ammo we need, and returns the amount that we can give.
    public int Consume(Type type, int amount)
    {
        switch (type)
        {
            case Type.Shotgun:
                return ConsumeAmmoType(ref shotgunAmmoReserve, amount);
            case Type.Pistol:
                return ConsumeAmmoType(ref pistolAmmoReserve, amount);
            case Type.Rifle:
                return ConsumeAmmoType(ref rifleAmmoReserve, amount);
            default:
                Debug.LogError("The currently selected weapon does not yet have an implemented ammo type.");
                return 0;
        }

        int ConsumeAmmoType(ref int reserve, int ammoToConsume)
        {
            ammoToConsume = Mathf.Min(ammoToConsume, reserve);
            reserve -= ammoToConsume;
            return ammoToConsume;
        }
    }

    public int GetReserve(Type type) => type switch
    {
        Type.Shotgun => shotgunAmmoReserve,
        Type.Pistol => pistolAmmoReserve,
        Type.Rifle => rifleAmmoReserve,
        _ => throw new System.NotImplementedException()
    };
}
