using UnityEngine;

public class UIWeaponList : MonoBehaviour
{
    [SerializeField]
    UIWeaponSelection[] weapons;
    [SerializeField]
    Sprite pistol, shotgun, rifle, launcher;

    private void Awake() {
        foreach( UIWeaponSelection weapon in weapons ) {
            weapon.Clear();
        }
    }

    public void UpdateList( GunData[] guns ) {
        for (int i = 0; i < weapons.Length; i++) {
            if(guns[i] == null)
                continue;
            switch(guns[i].type) {
                case GunType.Pistol:
                    weapons[i].SetWeaponImage(pistol, guns[i].GunQualityColor());
                    break;
                case GunType.Shotgun:
                    weapons[i].SetWeaponImage(shotgun, guns[i].GunQualityColor());
                    break;
                case GunType.Rifle:
                    weapons[i].SetWeaponImage(rifle, guns[i].GunQualityColor());
                    break;
                case GunType.Rocket:
                    weapons[i].SetWeaponImage(launcher, guns[i].GunQualityColor());
                    break;
            }
        }
    }

    public void UpdateSelected( int index ) {
        for (int i = 0; i < weapons.Length; i++) {
            if( i == index ) {
                weapons[i].OnSelected();
            } else {
                weapons[i].OnDeselected();
            }
        }
    }
}
