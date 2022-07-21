using UnityEngine;
using UnityEngine.UI;

public class UIWeaponSelection : MonoBehaviour
{
    [SerializeField]
    Image weaponImage, background;

    [SerializeField]
    Sprite selectedSprite, deselectedSprite;

    public void SetWeaponImage( Sprite sprite, Color tint ) {
        weaponImage.sprite = sprite;
        weaponImage.color = tint;
    }

    public void Clear() {
        weaponImage.sprite = null;
        weaponImage.color = Color.clear;
    }

    public void OnSelected() {
        background.sprite = selectedSprite;
    }

    public void OnDeselected() {
        background.sprite = deselectedSprite;
    }
}
