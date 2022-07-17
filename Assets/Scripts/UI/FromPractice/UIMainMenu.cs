using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{

    [SerializeField]
    UISlidingPanel optionsPanel;
    [SerializeField]
    UISlidingPanel creditsPanel;

    [SerializeField]
    string startScene;

    Selectable[] buttons;
    
    void Awake() {
        buttons = transform.Find("Sidebar").GetComponentsInChildren<Selectable>();
    }

    public void Start() {
        if( startScene != "" ) {
            SceneManager.LoadScene(startScene);
        }
    }

    public void OpenOptions() {
        optionsPanel.Toggle();
        creditsPanel.SlideOut();
    }
    public void OpenCredits() {
        creditsPanel.Toggle();
        optionsPanel.SlideOut();
    }
    public void Quit() {
        GameOptions.SaveOptions();
        Application.Quit();
    }

    public void EnableInteraction() {
        foreach(Selectable item in buttons) {
            item.interactable = true;
        }
    }

    public void DisableInteraction() {
        foreach(Selectable item in buttons) {
            item.interactable = false;
        }
    }

}
