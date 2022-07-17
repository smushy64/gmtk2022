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

    [SerializeField]
    AudioClip mainMenuMusic;

    Selectable[] buttons;
    
    void Start() {
        buttons = transform.Find("Sidebar").GetComponentsInChildren<Selectable>();
        MusicPlayer.Instance.PlayClip(mainMenuMusic);
    }

    public void StartGame() {
        if (startScene != "")
        {
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
