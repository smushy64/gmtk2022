using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    
    PlayerInput input;
    InputAction pauseAction;

    public static bool Paused = false;

    [SerializeField]
    GameObject pauseMenuPanel;

    private void Awake()
    {
        input = FindObjectOfType<PlayerInput>();
        pauseAction = input.actions["Pause"];
        Paused = false;
    }

    public void Pause() {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        pauseMenuPanel.SetActive(true);
    }

    public void UnPause() {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuPanel.SetActive(false);
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPauseAction(InputAction.CallbackContext ctx) {
        Paused = !Paused;
        if(Paused)
            Pause();
        else
            UnPause();
    }

    private void OnEnable()
    {
        pauseAction.started += OnPauseAction;
    }
    private void OnDisable()
    {
        pauseAction.started -= OnPauseAction;
        Time.timeScale = 1f;
    }

}
