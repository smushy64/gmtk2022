using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public LayerMask whatCanBeTouched;
    public int Damage;
    [SerializeField] private SoundEffectPlayer ExplosionSounds;

    private void Start()
    {
        ExplosionSounds.Play();
        Collider[] touching = Physics.OverlapSphere(this.transform.position, 3, whatCanBeTouched, QueryTriggerInteraction.Ignore);
        if(touching.Length > 0)
        {
            foreach (Collider item in touching)
            {
                print("touch");
                if (item.TryGetComponent(out Enemy enemy))
                {
                    print("enemy");
                    enemy.transform.gameObject.GetComponent<Enemy>().TakeDamage(Damage);

                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out PlayerHealth health))
        {
            if(collision.gameObject.tag == "PlayerExplosion")
                health.TakeDamage(Damage / 5);
            else
                health.TakeDamage(Damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3);

    }
}
