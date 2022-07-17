using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private int Damage;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(Damage);
        }
    }
}
