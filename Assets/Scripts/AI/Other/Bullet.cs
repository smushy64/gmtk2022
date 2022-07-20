using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask whatCanBeHit;
    public float Damage;
    private void OnCollisionEnter(Collision collision)
    {
        var health = collision.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            // health.TakeDamage(Mathf.RoundToInt(Damage));
        }

        Destroy(this.gameObject);
    }
}
