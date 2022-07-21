using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    GunData data;
    [SerializeField]
    GameObject[] guns;
    Renderer[] gunRenderers;
    MaterialPropertyBlock mpb;

    static WeaponManager weaponManager;
    bool isReturnedWeapon = false;

    private void Awake() {
        mpb = new MaterialPropertyBlock();
        gunRenderers = new Renderer[guns.Length];
        int i = 0;
        foreach(GameObject gun in guns) {
            gunRenderers[i] = gun.GetComponentInChildren<Renderer>();
            ++i;
        }
        if(weaponManager == null) {
            weaponManager = FindObjectOfType<WeaponManager>();
        }
    }

    public void Generate( Vector3 atPosition ) {
        transform.position = atPosition;
        SetData(new GunData());
        isReturnedWeapon = false;
    }

    public void SetData(GunData data) {
        this.data = data;
        mpb.SetColor("_EmissionColor", data.GunQualityColor() / 2f);
        foreach( GameObject gun in guns ) {
            gun.SetActive(false);
        }
        int index = (int)data.type;
        guns[index].SetActive(true);
        gunRenderers[index].SetPropertyBlock(mpb);
    }

    private void Start() {
        Generate(transform.position);
    }

    void PickedUp() {
        if(!isReturnedWeapon) {
            weaponManager.UpdateAmmoCount(data.type, data.magazineCapacity);
        }
        GunData returnWeapon = weaponManager.PickupWeapon(data);
        if(returnWeapon != null) {
            SetData(returnWeapon);
            isReturnedWeapon = true;
        }
        else {
            foreach( GameObject gun in guns ) {
                gun.SetActive(false);
            }
        }
    }

    public void OnInteract() {
        PickedUp();
    }
}
