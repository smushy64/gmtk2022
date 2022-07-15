using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask whatCanBeHit;
    private void OnCollisionEnter(Collision collision)
    {
        print("Hit");
        Destroy(this.gameObject);
    }
}
