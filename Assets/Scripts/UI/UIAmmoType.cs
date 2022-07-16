using UnityEngine;
using UnityEngine.UI;

public class UIAmmoType : MonoBehaviour
{
    Image image;
    [SerializeField]
    Sprite pistolImage, shotgunImage, rifleImage, rocketImage, energyImage;

    private void Awake() {
        image = GetComponent<Image>();
    }

    public void SetAmmoType( GunType type ) {
        image.sprite = GetImage(type);
        image.color = GetColor(type);
    }

    Sprite GetImage(GunType type) {
        switch( type ) {
            case GunType.Shotgun:
                return shotgunImage;
            case GunType.Rifle:
                return rifleImage;
            case GunType.Rocket:
                return rocketImage;
            case GunType.Energy:
                return energyImage;
            default: case GunType.Pistol:
                return pistolImage;
        }
    }

    Color GetColor(GunType type) {
        Color alpha = new Color(1f, 1f, 1f, 0.75f);
        switch( type ) {
            case GunType.Shotgun:
                return new Color( 0.839f, 0.1743f, 0.1544f ) * alpha;
            case GunType.Rifle:
                return new Color( 0.4978f, 1f, 0.0235f ) * alpha;
            case GunType.Rocket:
                return new Color( 1f, 0.5693f, 0.0235f ) * alpha;
            case GunType.Energy:
                return new Color( 0.0235f, 1f, 0.6593f ) * alpha;
            default: case GunType.Pistol:
                return new Color( 1f, 0.8905391f, 0.4481132f ) * alpha;
        }
    }

}
