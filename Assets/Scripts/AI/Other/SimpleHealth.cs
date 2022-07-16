using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleHealth : MonoBehaviour
{
    [SerializeField] float Health, MaxHealth;
    public UIHud UIH;
    public void TakeDamage(float damage)
    {
        Health -= damage;
        UIH.UpdateHealth(Health, MaxHealth);

        if (Health < 0)
            SceneManager.LoadScene("VurikAI");
    }
}
