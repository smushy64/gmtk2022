using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask whatCanBeHit;
    public float Damage;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<SimpleHealth>() != null)
            collision.gameObject.GetComponent<SimpleHealth>().TakeDamage(Damage);
        else if(collision.gameObject.GetComponentInParent<SimpleHealth>() != null)
            collision.gameObject.GetComponentInParent<SimpleHealth>().TakeDamage(Damage);

        Destroy(this.gameObject);
    }
}
