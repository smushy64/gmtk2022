using System;
using System.Collections;
using UnityEngine;

public class UISlidingPanel : MonoBehaviour
{

    public Action<bool> OnPanelSlide;

    public void SlideSidebar( bool slideIn ) {
        if( slide != null )
            StopCoroutine( slide );
        slide = Slide(slideIn);
        StartCoroutine(slide);
        OnPanelSlide?.Invoke(slideIn);
        IsInsideScreen = slideIn;
    }

    public void SlideOut() {
        if( IsInsideScreen ) {
            SlideSidebar(false);
        }
    }

    public void Toggle() {
        SlideSidebar(!IsInsideScreen);
    }

    [SerializeField]
    bool slideInOnStart = false;
    [SerializeField]
    bool reverse = false;
    [SerializeField]
    bool slideInFromRight = false;
    [SerializeField, Range(0f, 2f)]
    float animationLength = 0.25f;
    [SerializeField]
    Vector3 insideOffset = Vector3.zero;
    [SerializeField]
    Vector3 outsideOffset = Vector3.zero;

    public bool IsInsideScreen{ get; private set; } = false;

    RectTransform rectTransform;
    float width;
    Vector3 outsidePosition;
    Vector3 insidePosition = Vector3.zero;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.sizeDelta.x;
        outsidePosition = new Vector3(slideInFromRight ? width : -width, 0f, 0f) + outsideOffset;
        insidePosition += insideOffset;
    }

    void Start() {
        if( slideInOnStart )
            SlideSidebar(true);
    }

    IEnumerator slide;
    IEnumerator Slide( bool slideIn ) {
        float timer = 0f;
        Vector3 start = slideIn ? outsidePosition : insidePosition;
        Vector3 end = slideIn ? insidePosition : outsidePosition;
        if( reverse ) {
            Vector3 temp = start;
            start = end;
            end = temp;
        }
        while( timer < animationLength ) {
            timer += Time.unscaledDeltaTime;
            rectTransform.anchoredPosition3D = Vector3.Lerp(start, end, timer / animationLength);
            yield return null;
        }
        rectTransform.anchoredPosition3D = end;
    }

}
