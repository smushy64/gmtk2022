using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UISlider : MonoBehaviour
{

    [SerializeField]
    ValueDisplayType displayType = ValueDisplayType.Value;

    public enum ValueDisplayType {
        Percentage,
        Value,
    }

    TMP_Text valueLabel;
    public Slider SliderComponent { get; private set; }

    private void Awake()
    {
        valueLabel = transform.Find("Value").GetComponent<TMP_Text>();
        SliderComponent = GetComponentInChildren<Slider>();
        SliderComponent.onValueChanged.AddListener(delegate { UpdateLabel(); });
    }

    void UpdateLabel() {
        switch(displayType) {
            case ValueDisplayType.Percentage:
                valueLabel.text = ( (int)( SliderComponent.value * 100f ) ).ToString();
                break;
            default: case ValueDisplayType.Value:
                valueLabel.text = SliderComponent.value.ToString("F1");
                break;
        }
    }

    public void SetValue( float value ) {
        SliderComponent.value = value;
        UpdateLabel();
    }

    public void SetInteractable(bool interactable) {
        SliderComponent.interactable = interactable;
    }

}
