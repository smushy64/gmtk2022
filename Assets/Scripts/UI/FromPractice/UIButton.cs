using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton :
    MonoBehaviour,
    ISelectHandler, IDeselectHandler,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    ISubmitHandler
{

    RectTransform button;
    bool selected = false;

    float deselectSize = 1f;
    [SerializeField, Range(1f, 2f)]
    float selectSize = 1.06f;
    [SerializeField, Range(0f, 1f)]
    float submitSize = 0.9f;
    [SerializeField, Range(0f, 2f)]
    float animationLength = 0.15f;

    [SerializeField]
    SoundEffectPlayer sfxPlayer;

    void Selected() {
        selected = true;
        StopScale();
        scaleButton = ScaleButton(Vector3.one * selectSize);
        StartCoroutine(scaleButton);
    }

    void Deselected() {
        selected = false;
        CursorManager.SetCursor(CursorManager.CursorType.Normal);
        StopScale();
        scaleButton = ScaleButton(Vector3.one * deselectSize);
        StartCoroutine(scaleButton);
    }

    void Submit() {
        StopScale();
        scaleButtonBack = ScaleButtonBack(Vector3.one * submitSize);
        StartCoroutine(scaleButtonBack);
        sfxPlayer?.Play();
    }

    void PointerDown() {
        StopScale();
        scaleButton = ScaleButton(Vector3.one * submitSize);
        StartCoroutine(scaleButton);
        sfxPlayer?.Play();
    }

    void PointerUp() {
        StopScale();

        Vector3 targetSize = selected ? Vector3.one * selectSize : Vector3.one * deselectSize;

        scaleButton = ScaleButton(targetSize);
        StartCoroutine(scaleButton);
    }

    void StopScale() {
        if( scaleButton != null ) {
            StopCoroutine(scaleButton);
        }
        if( scaleButtonBack != null ) {
            StopCoroutine(scaleButtonBack);
        }
    }

    IEnumerator scaleButton;
    IEnumerator ScaleButton(Vector3 endScale) {
        float timer = 0f;
        Vector3 startScale = button.transform.localScale;
        while( timer < animationLength ) {

            button.transform.localScale = Vector3.Lerp(
                startScale,
                endScale,
                timer / animationLength
            );

            timer += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator scaleButtonBack;
    IEnumerator ScaleButtonBack(Vector3 endScale) {
        float timer = 0f;
        Vector3 startScale = button.transform.localScale;
        while( timer < animationLength ) {

            button.transform.localScale = Vector3.Lerp(
                startScale,
                endScale,
                timer / animationLength
            );

            timer += Time.deltaTime;

            yield return null;
        }
        timer = 0f;
        while( timer < animationLength ) {

            button.transform.localScale = Vector3.Lerp(
                endScale,
                Vector3.one * selectSize,
                timer / animationLength
            );

            timer += Time.deltaTime;

            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        CursorManager.SetCursor(CursorManager.CursorType.Link);
        Selected();
    }
    public void OnSelect(BaseEventData eventData) => Selected(); 

    public void OnPointerExit(PointerEventData eventData) {
        CursorManager.SetCursor(CursorManager.CursorType.Normal);
        Deselected();
    }
    public void OnDeselect(BaseEventData eventData) => Deselected();

    public void OnPointerDown(PointerEventData eventData) => PointerDown();
    public void OnPointerUp(PointerEventData eventData) => PointerUp();
    public void OnSubmit(BaseEventData eventData) => Submit();

    void Awake() => button = GetComponent<RectTransform>();
    
}
