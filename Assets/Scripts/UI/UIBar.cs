using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{

    [SerializeField]
    Color backgroundColor = Color.gray;
    [SerializeField]
    Color fillColor = Color.white;
    [SerializeField, Range(0f,1f)]
    float startValue = 0.5f;
    [SerializeField]
    float maxFillSize = 600f;
    [SerializeField]
    bool scaleWidth = true;

    public float Value { get; private set; } = 0.5f;

    public void SetValue(float value) 
    { 
        Value = Mathf.Clamp01(value);
        RecalculateFill();
    }

    Image backgroundImage;
    Image fillImage;

    void Initialize() {
        backgroundImage = transform.Find( "Background" ).GetComponent<Image>();
        fillImage = backgroundImage.transform.GetChild(0).GetComponent<Image>();
        backgroundImage.color = backgroundColor;
        fillImage.color = fillColor;
        Value = startValue;
        RecalculateFill();
    }

    void RecalculateFill() {
        fillImage.rectTransform.sizeDelta = scaleWidth ?
        new Vector2(
            maxFillSize * Value,
            fillImage.rectTransform.sizeDelta.y
        ) :
        new Vector2(
            fillImage.rectTransform.sizeDelta.x,
            maxFillSize * Value
        );
    }

    void OnValidate() => Initialize();
    void Awake() => Initialize();

}
