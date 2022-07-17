using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIPopUp : MonoBehaviour
{

    Image background;
    RectTransform popUpWindow;
    TMP_Text message;

    TMP_Text leftButtonText;
    TMP_Text rightButtonText;

    Button leftButton;
    Button rightButton;

    EventSystem eventSystem;

    void Start() {

        eventSystem = EventSystem.current;

        background = transform.Find("Background").GetComponent<Image>();
        popUpWindow = transform.Find("PopupPanel").GetComponent<RectTransform>();
        Transform popUpLayout = popUpWindow.GetChild(0);
        message = popUpLayout.Find("Message").GetComponent<TMP_Text>();
        Transform buttonArea = popUpLayout.Find("ButtonArea");
        leftButtonText = buttonArea.GetChild(0).GetComponentInChildren<TMP_Text>();
        rightButtonText = buttonArea.GetChild(1).GetComponentInChildren<TMP_Text>();

        leftButton = buttonArea.GetChild(0).GetComponent<Button>();
        rightButton = buttonArea.GetChild(1).GetComponent<Button>();

        leftButton.onClick.AddListener(delegate { OnLeftButtonSubmit(); });
        rightButton.onClick.AddListener(delegate { OnRightButtonSubmit(); });
    }

    Action leftButtonAction;
    Action rightButtonAction;
    Action dismissAction;

    void OnLeftButtonSubmit() => leftButtonAction?.Invoke();
    void OnRightButtonSubmit() => rightButtonAction?.Invoke();

    public void CreatePopup(
        GameObject lastSelected,
        string message,
        string leftButtonMessage,
        string rightButtonMessage,
        Action leftButtonAction,
        Action rightButtonAction
    ) {
        this.leftButtonAction  = leftButtonAction;
        this.rightButtonAction = rightButtonAction;
        this.message.text = message;
        leftButtonText.text = leftButtonMessage;
        rightButtonText.text = rightButtonMessage;

        leftButton.interactable  = true;
        rightButton.interactable = true;

        this.lastSelected = lastSelected;

        SelectUIElement(leftButton.gameObject);

        Pop(true);
    }

    public void CreatePopup(
        GameObject lastSelected,
        string message,
        string leftButtonMessage,
        string rightButtonMessage,
        Action leftButtonAction,
        Action rightButtonAction,
        float dismissTimer,
        Action dismissAction
    ) {
        this.dismissAction = dismissAction;
        CreatePopup(lastSelected, message, leftButtonMessage, rightButtonMessage, leftButtonAction, rightButtonAction);
        if( dismissTimed != null ) {
            StopCoroutine(dismissTimed);
        }
        dismissTimed = DismissTimed(dismissTimer);
        StartCoroutine(dismissTimed);
    }

    GameObject lastSelected;
    void Pop( bool up ) {
        if( popAnimation != null ) {
            StopCoroutine(popAnimation);
        }
        popAnimation = PopAnimation(up);
        StartCoroutine(popAnimation);
    }

    float animationLength = 0.15f;
    IEnumerator popAnimation;
    IEnumerator PopAnimation( bool up ) {
        float timer = 0f;
        Color fadeStart = up ? Color.clear : new Color( 0f, 0f, 0f, 0.8f );
        Color fadeEnd = up ? new Color( 0f, 0f, 0f, 0.8f ) : Color.clear;

        Vector3 popUpClosedSize = Vector3.zero;
        Vector3 popUpOpenedSize = Vector3.one;

        Vector3 popUpSizeStart = up ? popUpClosedSize : popUpOpenedSize;
        Vector3 popUpSizeEnd = up ? popUpOpenedSize : popUpClosedSize;

        if( up )
            background.gameObject.SetActive(true);

        while( timer < animationLength ) {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / animationLength);

            popUpWindow.localScale = Vector3.Lerp(
                popUpSizeStart,
                popUpSizeEnd,
                t
            );

            background.color = Color.Lerp(
                fadeStart,
                fadeEnd,
                t
            );

            yield return null;
        }

        if( !up )
            background.gameObject.SetActive(false);
    }

    IEnumerator dismissTimed;
    IEnumerator DismissTimed( float time ) {
        float timer = 0f;
        while( timer < time ) {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        dismissAction?.Invoke();
        DismissPopup();
    }

    public void DismissPopup() {
        if( dismissTimed != null ) {
            StopCoroutine(dismissTimed);
        }
        leftButtonAction = null;
        rightButtonAction = null;
        dismissAction = null;
        leftButton.interactable = false;
        rightButton.interactable = false;

        SelectUIElement(lastSelected);
        Pop(false);
    }


    void SelectUIElement( GameObject objectToSelect ) {
        if( selectUIElementCR != null ) {
            StopCoroutine(selectUIElementCR);
        }
        selectUIElementCR = SelectUIElementCR( objectToSelect );
        StartCoroutine(selectUIElementCR);
    }
    IEnumerator selectUIElementCR;
    IEnumerator SelectUIElementCR( GameObject objectToSelect ) {
        eventSystem.SetSelectedGameObject(null);
        int i = 0;
        while( i < 1 ) {
            i++;
            yield return null;
        }
        eventSystem.SetSelectedGameObject(objectToSelect);
    }

}
