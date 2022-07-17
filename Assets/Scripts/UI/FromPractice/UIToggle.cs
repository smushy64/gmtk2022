using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour
{

    [SerializeField]
    string trueText = "True";
    [SerializeField]
    string falseText = "False";
    [SerializeField]
    public UnityEvent<bool> OnToggle;

    TMP_Text label;
    Button button;

    public bool IsTrue{ get; private set; } = false;

    private void Awake() {
        button = GetComponentInChildren<Button>();
        label = button.GetComponentInChildren<TMP_Text>();
        button.onClick.AddListener( delegate{ Toggle(); } );
    }

    private void OnValidate()
    {
        if( label == null ) {
            button = GetComponentInChildren<Button>();
            label = button.GetComponentInChildren<TMP_Text>();
        }
        UpdateText();
    }

    void Toggle() {
        Set(!IsTrue);
        OnToggle?.Invoke(IsTrue);
    }

    public void Set(bool value) {
        IsTrue = value;
        UpdateText();
    }

    void UpdateText() {
        label.text = IsTrue ? trueText : falseText;
    }

    public void SetInteractable( bool interactable ) {
        button.interactable = interactable;
    }
}
