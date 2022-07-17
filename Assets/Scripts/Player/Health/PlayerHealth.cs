using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private UIHud hud;
    private int currentHealth;

    private WaveManager waveManager;

    private void Awake()
    {
        waveManager = FindObjectOfType<WaveManager>();
        waveManager.onWaveEnded += _ => HealPlayer();
    }

    private void Start()
    {
        hud.UpdateHealth(currentHealth, maxHealth);
    }

    public void HealPlayer()
    {
        currentHealth = maxHealth;
        hud.UpdateHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        hud.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }
}
