using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float Damage;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<SimpleHealth>() != null)
            collision.gameObject.GetComponent<SimpleHealth>().TakeDamage(Damage);
        else if (collision.gameObject.GetComponentInParent<SimpleHealth>() != null)
            collision.gameObject.GetComponentInParent<SimpleHealth>().TakeDamage(Damage);
    }
}
