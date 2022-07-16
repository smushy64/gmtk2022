using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float Health;
    [SerializeField] private GameObject DieparticleOrExplosion;

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Instantiate(DieparticleOrExplosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
