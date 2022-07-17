using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOver : MonoBehaviour
{

    [SerializeField]
    GameObject menu;

    SoundEffectPlayer sfxPlayer;

    private void Start()
    {
        sfxPlayer = GetComponentInChildren<SoundEffectPlayer>();
        GameOverManager.Instance.OnPlayerDeath += Activate;
    }
    private void OnDisable()
    {
        GameOverManager.Instance.OnPlayerDeath -= Activate;
    }

    public void Activate() {
        sfxPlayer.Play();
        FindObjectOfType<PauseMenu>().gameObject.SetActive(false);
        menu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        PauseMenu.Paused = true;
    }

    public void Retry() {
        Time.timeScale = 1f;
        PauseMenu.Paused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void MainMenu() {
        Time.timeScale = 1f;
        PauseMenu.Paused = false;
        SceneManager.LoadScene("MainMenu");
    }

}
