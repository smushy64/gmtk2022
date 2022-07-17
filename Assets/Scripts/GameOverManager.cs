using UnityEngine;
using System;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    private void Awake()
    {
        if( Instance != null && Instance != this ) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    public void PlayerDeath() {
        Time.timeScale = 0f;
        OnPlayerDeath?.Invoke();
    }

    public Action OnPlayerDeath;

}
